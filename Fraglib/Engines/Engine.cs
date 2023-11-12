using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace Fraglib;

internal abstract class Engine : GameWindow {
    public Engine(int width, int height, string title) : base(
        GameWindowSettings.Default,
        new NativeWindowSettings() {
            Size = (width, height),
            Title = title,
            StartVisible = false,
            StartFocused = true,
            MinimumSize = (width, height),
            MaximumSize = (width, height),
        }) {
        CenterWindow();

        Screen = new uint[width * height];
        WindowHeight = height;
        WindowWidth = width;
        WindowTitle = title;
    }
    
    public readonly int WindowHeight, WindowWidth;
    public readonly uint[] Screen;
    public readonly string WindowTitle;

    private int programHandle;
    private int vertexArrayHandle;
    private int textureHandle;
    private int vertexBufferHandle;
    private int indexBufferHandle;

    public float ElapsedTime { get; private set; } = 0f;
    public float DeltaTime { get; private set; } = 0f;
    public bool VSyncEnabled { get => VSync == VSyncMode.On; set => VSync = value ? VSyncMode.On : VSyncMode.Off; }
    public int TargetFramerate { get; set; } = 144;
    public int PixelSize { get; set; } = 1;
    public ScaleType ScaleType { get; set; } = ScaleType.None;

    private float frameTimer = 0;

    protected override void OnLoad() {
        base.OnLoad();

        IsVisible = true;

        float[] vertices = {
            -1f, -1f,
            1f, -1f,
            1f, 1f,
            -1f, 1f
        };

        int[] indices = {
            0, 1, 2,
            0, 2, 3
        };

        vertexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        indexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        vertexArrayHandle = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayHandle);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        string vertexShaderSource = @"
            #version 330

            layout(location = 0) in vec2 position;

            void main() {
                gl_Position = vec4(position, 0.0, 1.0);
            }
        ";
        string fragmentShaderSource = @$"
            #version 330

            uniform sampler2D textureSampler;

            out vec4 fragColor;

            void main() {{
                {(PixelSize == 1 || this is PerPixelEngine ? 
                    "fragColor = texture(textureSampler, gl_FragCoord.xy / textureSize(textureSampler, 0));" : 

                    ScaleType == ScaleType.Average ? 
                    @$"
                        vec2 pixelCoord = floor(gl_FragCoord.xy / vec2({PixelSize}, {PixelSize})) * vec2({PixelSize}, {PixelSize});
                        vec2 texCoords = pixelCoord / textureSize(textureSampler, 0);
                        vec3 averageColor = vec3(0.0);

                        for (int i = 0; i < {PixelSize}; i++) {{
                            for (int j = 0; j < {PixelSize}; j++) {{
                                texCoords = (pixelCoord + vec2(i, j)) / textureSize(textureSampler, 0);
                                averageColor += texture(textureSampler, texCoords).rgb;
                            }}
                        }}

                        averageColor /= float({PixelSize} * {PixelSize});
                        fragColor = vec4(averageColor, 1.0);
                    " : 
                    @$"
                        vec2 pixelCoord = floor(gl_FragCoord.xy / {PixelSize}.0) * {PixelSize}.0;
                        vec2 texCoords = pixelCoord / textureSize(textureSampler, 0);
                        fragColor = texture(textureSampler, texCoords);
                    "
                )}
            }}
        ";
        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);


        programHandle = GL.CreateProgram();
        GL.AttachShader(programHandle, vertexShader);
        GL.AttachShader(programHandle, fragmentShader);
        GL.LinkProgram(programHandle);
        GL.DetachShader(programHandle, vertexShader);
        GL.DetachShader(programHandle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        textureHandle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureHandle);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ClientSize.X, ClientSize.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    protected override void OnUnload() {
        base.OnUnload();

        OnWindowClose();

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(vertexArrayHandle);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(indexBufferHandle);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferHandle);

        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.DeleteTexture(textureHandle);

        GL.UseProgram(0);
        GL.DeleteProgram(programHandle);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindTexture(TextureTarget.Texture2D, textureHandle);
        unsafe {
            fixed (uint* ptr = Screen) {
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, ClientSize.X, ClientSize.Y, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)ptr);
            }
        }

        GL.UseProgram(programHandle);
        GL.BindVertexArray(vertexArrayHandle);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        float t = (float)args.Time;
        DeltaTime = t;
        ElapsedTime += t;

        if (VSync == VSyncMode.On) {
            Update(args);
            Title = $"{WindowTitle} | FPS: {1f / t:F0}";
            return;
        }

        frameTimer += t;
        if (frameTimer >= 1f / TargetFramerate) {
            Update(args);
            Title = $"{WindowTitle} | FPS: {1f / frameTimer:F0}";
            frameTimer = 0f;
        }
    }

    private int CompileShader(ShaderType type, string source) {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int compileStatus);

        if (compileStatus != 1) {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling {type}: {infoLog}");
        }

        return shader;
    }

    public abstract void Update(FrameEventArgs args);
    public abstract void OnWindowClose();
}

public enum ScaleType {
    None,
    Average
}