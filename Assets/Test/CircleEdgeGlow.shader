Shader "Custom/CircleOuterGlow"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1, 0.8, 0.3, 1)
        _GlowIntensity("Glow Intensity", Range(0, 5)) = 1.0
        _GlowSpread("Glow Spread", Range(0.01, 0.5)) = 0.1
        _GlowRadius("Glow Radius", Range(0.0, 1.0)) = 0.45
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Stencil {
                Ref 1
                Comp Equal
                Pass Keep
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowSpread;
            float _GlowRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);
                float dist = distance(uv, center);

                fixed4 texCol = tex2D(_MainTex, uv);

                // Tính glow từ rìa vòng phát ra ngoài
                float inner = _GlowRadius;
                float outer = _GlowRadius + _GlowSpread;
                float glow = smoothstep(outer, inner, dist);

                float3 glowColor = _GlowColor.rgb * glow * _GlowIntensity;

                float alpha = texCol.a + glow * _GlowColor.a * _GlowIntensity;

                float3 finalColor = texCol.rgb + glowColor;

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
