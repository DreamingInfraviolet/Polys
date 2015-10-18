#version 130
attribute vec4 vertuv;
varying vec2 uv;
uniform float screenWidth;
uniform float screenHeight;
uniform float sourceWidth;
uniform float sourceHeight;
uniform float uvMultiplier;

void main()
{
        float aspecti = sourceWidth/sourceHeight;
        float aspectr = screenWidth/screenHeight;

        vec2 scale = vec2(1,1);
        if(aspectr<aspecti)
            scale.y = aspectr/aspecti;
        else
            scale.x = aspecti/aspectr;
      
    gl_Position = vec4(vertuv.xy*scale,0,1);

uv = vec2(vertuv.z, uvMultiplier*vertuv.w);
}