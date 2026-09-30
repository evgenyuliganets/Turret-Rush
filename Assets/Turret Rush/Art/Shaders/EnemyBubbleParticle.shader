Shader "TurretRush/EnemyBubbleParticle"
{
    Properties
    {
        [HDR] _BaseColor ("Base Color", Color) = (1, 0.05, 0.02, 1)
        [HDR] _EdgeColor ("Edge Color", Color) = (1, 0.35, 0.08, 1)

        _Softness ("Edge Softness", Range(0.001, 0.5)) = 0.08
        _RimWidth ("Rim Width", Range(0.001, 0.5)) = 0.12
        _RimStrength ("Rim Strength", Range(0, 3)) = 0.6
        _Emission ("Emission", Range(0, 5)) = 1.25

        _Alpha ("Global Alpha", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EdgeColor;
                half _Softness;
                half _RimWidth;
                half _RimStrength;
                half _Emission;
                half _Alpha;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);

                output.positionHCS = positionInputs.positionCS;
                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Particle quad UV (0..1) -> centered circle coordinates (-1..1).
                float2 p = input.uv * 2.0 - 1.0;
                float dist = length(p);

                // Soft circular mask.
                half circle =
                    1.0h - smoothstep(
                        1.0h - _Softness,
                        1.0h,
                        (half)dist
                    );

                // A subtle bright ring near the outside of the blob.
                half innerMask =
                    1.0h - smoothstep(
                        1.0h - _RimWidth - _Softness,
                        1.0h - _RimWidth,
                        (half)dist
                    );

                half rim =
                    saturate(circle - innerMask);

                half3 baseColor =
                    _BaseColor.rgb * input.color.rgb;

                half3 rimColor =
                    _EdgeColor.rgb * input.color.rgb;

                half3 rgb =
                    lerp(
                        baseColor,
                        rimColor,
                        saturate(rim * _RimStrength)
                    );

                rgb *= _Emission;

                half alpha =
                    circle *
                    _BaseColor.a *
                    input.color.a *
                    _Alpha;

                return half4(rgb, alpha);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
