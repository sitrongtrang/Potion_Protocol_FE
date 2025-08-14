Shader "Custom/SoftCircleGlowV2"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1, 0.8, 0.3, 1)
        _GlowIntensity("Glow Intensity", Float) = 1.0
        _GlowRange("Glow Range", Range(0.001, 0.5)) = 0.1
        _CircleRadius("Circle Radius", Range(0.0, 0.5)) = 0.4
        _Stencil("Stencil ID", Float) = 0
        _StencilComp("Stencil Comparison", Float) = 8        // Always
        _StencilOp("Stencil Operation", Float) = 0           // Keep
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            ColorMask [_ColorMask]
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowRange;
            float _CircleRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);
                float dist = distance(uv, center);

                // Inner glow (fade in)
                float innerGlow = smoothstep(_CircleRadius - _GlowRange, _CircleRadius, dist);

                // Outer glow (fade out)
                float outerGlow = 1.0 - smoothstep(_CircleRadius, _CircleRadius + _GlowRange, dist);

                // Core glow (trong sáng mờ nhẹ)
                float coreGlow = 1.0 - smoothstep(0.0, _CircleRadius - _GlowRange * 0.5, dist);

                float combinedGlow = (innerGlow * outerGlow) + coreGlow * 0.2;

                fixed4 texCol = tex2D(_MainTex, uv);
                float3 finalColor = lerp(texCol.rgb, _GlowColor.rgb, combinedGlow * _GlowIntensity);
                float alpha = texCol.a + combinedGlow * _GlowIntensity;

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
