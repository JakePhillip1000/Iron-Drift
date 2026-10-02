Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color("Base Color", Color) = (1, 1, 1, 1)
        _MainTex("Base Texture", 2D) = "white" { }
        _LightColor("Light Color", Color) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
            Pass
            {
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes
                {
                    float4 position : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 position : SV_POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                float4 _Color;
                float _LightColor;

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    o.position = TransformObjectToHClip(v.position);
                    o.normal = normalize(v.normal);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(Varyings i) : SV_Target
                {
                    float light = max(0, dot(i.normal, float3(0, 0, 1))); // simple lighting
                    light = (light > 0.5) ? 1.0 : 0.2; 

                    return _Color * light;
                }
                ENDHLSL
            }
        }
            FallBack "Diffuse"
}