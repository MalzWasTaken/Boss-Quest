Shader "Custom/WarpShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WarpAmount ("Warp Amount", Float) = 0
        _BlurAmount ("Blur Amount", Float) = 0
        _ChromaAmount ("Chromatic Aberration", Float) = 0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _WarpAmount;
            float _BlurAmount;
            float _ChromaAmount;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);
                float2 dir = uv - center;
                float dist = length(dir);

                // Warp - stretch from center outward
                float2 warpedUV = uv + dir * dist * _WarpAmount;

                // Chromatic aberration - split RGB channels
                float2 redUV = warpedUV + dir * _ChromaAmount;
                float2 blueUV = warpedUV - dir * _ChromaAmount;

                float r = tex2D(_MainTex, redUV).r;
                float g = tex2D(_MainTex, warpedUV).g;
                float b = tex2D(_MainTex, blueUV).b;

                // Blur - sample multiple times
                float4 blurred = float4(0,0,0,0);
                int samples = 8;
                for(int j = 0; j < samples; j++)
                {
                    float angle = j * 3.14159 * 2.0 / samples;
                    float2 offset = float2(cos(angle), sin(angle)) * _BlurAmount * dist;
                    blurred += tex2D(_MainTex, warpedUV + offset);
                }
                blurred /= samples;

                float4 col = float4(r, g, b, 1);
                return lerp(col, blurred, _BlurAmount * 5);
            }
            ENDCG
        }
    }
}