#version 130
varying vec2 uv;
uniform sampler2D diffuse;  
uniform float time;
vec2 amount = vec2(0.009, 0.009)*sin(time*0.01);

void main()
{
    float r = texture(diffuse, uv+amount).r;
    float g = texture(diffuse, uv).g;
    float b = texture(diffuse, uv-amount).b;
    float a = texture(diffuse, uv).a;
    gl_FragColor = vec4(r,g,b,a);
}