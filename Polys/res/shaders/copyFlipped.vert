#version 130
attribute vec4 vertuv;
varying vec2 uv;
void main()
{
    gl_Position = vec4(vertuv.xy,0,1);             
    uv = vec2(vertuv.z, -vertuv.w);
}