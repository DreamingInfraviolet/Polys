#version 130
attribute vec4 vertuv;
varying vec2 uv;
uniform mat4 orthoMatrix;
uniform mat4 uvMatrix;
void main()
{
    gl_Position = orthoMatrix*vec4(vertuv.x,vertuv.y,0,1);             
    uv = (uvMatrix*vec4(vec2(vertuv.z, vertuv.w), 0, 1)).xy;
}