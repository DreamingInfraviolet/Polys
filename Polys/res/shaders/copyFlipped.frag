#version 130
varying vec2 uv;
uniform sampler2D diffuse;

void main()
{
    gl_FragColor = texture(diffuse, uv);
}