Shader "Custom/CrystalShield"
{
    Properties
    {
        _BaseColor      ("Base Color",       Color)  = (0.25, 0.75, 1.0, 0.28)
        _EmissionColor  ("Emission Color",   Color)  = (0.1, 0.55, 1.0, 1.0)
        _EmissionPower  ("Emission Power",   Float)  = 1.8
        _PatternScale   ("Pattern Scale",    Float)  = 6.0
        _PatternSpeed   ("Pattern Speed",    Float)  = 1.4
        _PatternSharp   ("Pattern Sharpness",Float)  = 0.55
        _VertexWave     ("Vertex Wave",      Float)  = 0.04
        _WaveSpeed      ("Wave Speed",       Float)  = 2.5
        _FresnelPower   ("Fresnel Power",    Float)  = 2.5
        _ColorShift     ("Color Shift Speed",Float)  = 0.3
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent+1"
        }

        Pass
        {
            Name "CrystalShieldPass"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _EmissionColor;
                float  _EmissionPower;
                float  _PatternScale;
                float  _PatternSpeed;
                float  _PatternSharp;
                float  _VertexWave;
                float  _WaveSpeed;
                float  _FresnelPower;
                float  _ColorShift;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 viewDirWS  : TEXCOORD1;
                float2 uv         : TEXCOORD2;
                float3 posWS      : TEXCOORD3;
            };

            // Pseudo-random based on vec2
            float rand2(float2 co)
            {
                return frac(sin(dot(co, float2(127.1, 311.7))) * 43758.5453);
            }

            // Hexagonal tiling distance field
            float hexPattern(float2 uv, float scale, float t)
            {
                uv *= scale;
                // offset rows
                float2 g  = float2(uv.x + sin(uv.y * 1.5 + t) * 0.3,
                                   uv.y + cos(uv.x * 1.5 + t) * 0.3);
                float2 h  = frac(g) - 0.5;
                float  d  = length(h);
                return smoothstep(_PatternSharp + 0.05, _PatternSharp, d);
            }

            // Geometric triangle grid
            float triGrid(float2 uv, float scale, float t)
            {
                uv *= scale;
                uv += float2(t * 0.4, -t * 0.25);
                float2 g = frac(uv + float2(sin(uv.y + t), cos(uv.x + t)) * 0.12) - 0.5;
                float  d = abs(g.x) + abs(g.y);
                return smoothstep(0.48, 0.35, d);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float t = _Time.y * _WaveSpeed;

                // Animated vertex displacement along normal (crystal breathing)
                float wave = sin(IN.positionOS.x * 4.0 + IN.positionOS.y * 3.5 + t) *
                             cos(IN.positionOS.z * 4.0 + t * 0.7);
                float3 displaced = IN.positionOS.xyz + IN.normalOS * (wave * _VertexWave);

                VertexPositionInputs posInputs = GetVertexPositionInputs(displaced);
                VertexNormalInputs   nmInputs  = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.normalWS   = nmInputs.normalWS;
                OUT.viewDirWS  = GetWorldSpaceViewDir(posInputs.positionWS);
                OUT.uv         = IN.uv;
                OUT.posWS      = posInputs.positionWS;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y;

                // --- Fresnel rim ---
                float3 N       = normalize(IN.normalWS);
                float3 V       = normalize(IN.viewDirWS);
                float  fresnel = pow(1.0 - saturate(dot(N, V)), _FresnelPower);

                // --- Geometric patterns (hex + tri layered) ---
                float2 uv      = IN.uv;
                float  hexA    = hexPattern(uv, _PatternScale,          t * _PatternSpeed);
                float  hexB    = hexPattern(uv, _PatternScale * 1.618,  t * _PatternSpeed * 0.7);
                float  tri     = triGrid   (uv, _PatternScale * 0.9,    t * _PatternSpeed * 0.5);

                float  pattern = saturate(hexA * 0.55 + hexB * 0.3 + tri * 0.35);

                // --- Animated color shift ---
                float  hueT    = t * _ColorShift;
                float3 colorA  = float3(0.1 + 0.15 * sin(hueT),
                                        0.65 + 0.2  * cos(hueT * 1.3),
                                        1.0);
                float3 colorB  = float3(0.5 + 0.4 * cos(hueT * 0.7),
                                        0.2 + 0.3 * sin(hueT * 1.1),
                                        0.9 + 0.1 * cos(hueT * 2.0));
                float3 dynColor = lerp(colorA, colorB, pattern);

                // Blend with the inspector base color
                float3 baseRGB = _BaseColor.rgb;
                float3 finalRGB = lerp(baseRGB, dynColor, 0.65);

                // --- Emission on pattern lines + rim ---
                float3 emiss   = _EmissionColor.rgb * _EmissionPower *
                                 (pattern * 0.8 + fresnel * 1.2);

                finalRGB += emiss;

                // --- Alpha: base + fresnel + pattern edges ---
                float alpha = _BaseColor.a
                            + fresnel  * 0.45
                            + pattern  * 0.18;
                alpha = saturate(alpha);

                return half4(finalRGB, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
