Shader "Custom/SpriteWithRingAndCenterGlow"
{
    Properties
    {
        _MainTex("Sprite", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1,1,0,1)
        _GlowRadius("Glow Radius", Range(0, 1)) = 0.15
        _GlowIntensity("Glow Intensity", Range(0.1, 10)) = 3.0
        _GlowCenterRadius("Center Radius", Range(0, 1)) = 0.4
        _GlowCenterStrength("Center Strength", Range(0.1, 10)) = 4.0
        _RingPosition("Ring Position (0=center)", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _GlowColor;
            float _GlowRadius;
            float _GlowIntensity;
            float _GlowCenterRadius;
            float _GlowCenterStrength;
            float _RingPosition;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 centeredUV = i.uv * 2.0 - 1.0;
                float dist = length(centeredUV);

                fixed4 col = tex2D(_MainTex, i.uv);
                float spriteAlpha = col.a;

                // Loại bỏ glow ở vùng sprite thật (alpha > threshold)
                float mask = smoothstep(0.03, 0.1, spriteAlpha);

                // ==== Glow Viền ====
                float ring = abs(dist - _RingPosition);
                float ringGlow = saturate(1.0 - ring / _GlowRadius);
                ringGlow = pow(ringGlow, _GlowIntensity);
                ringGlow *= (1.0 - mask);

                // ==== Glow Tâm ====
                float centerGlow = saturate(1.0 - dist / _GlowCenterRadius);
                centerGlow = pow(centerGlow, _GlowCenterStrength);
                centerGlow *= (1.0 - mask);

                float totalGlow = ringGlow + centerGlow;

                fixed3 finalRGB = col.rgb + _GlowColor.rgb * totalGlow;
                float finalA = max(spriteAlpha, totalGlow * _GlowColor.a);

                return fixed4(finalRGB, finalA);
            }
            ENDCG
        }
    }
}
