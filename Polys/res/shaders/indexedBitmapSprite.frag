#version 130
varying vec2 uv;
uniform sampler2D indexTexture;
uniform sampler1D paletteTexture;

void main()
{
    gl_FragColor = texture(paletteTexture, texture(indexTexture, uv).r);
}