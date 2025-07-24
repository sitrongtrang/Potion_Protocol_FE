Shader "Custom/SpriteWithRingGlow"
{
    Properties
    {
        _MainTex("Sprite", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1,1,0,1)
        _GlowRadius("Glow Radius", Range(0, 1)) = 0.2
        _GlowIntensity("Glow Intensity", Range(0.1, 5)) = 2.0
        _GlowCenterRadius("Glow Center Radius", Range(0, 1)) = 0.5
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

                // Ring glow centered at _GlowCenterRadius
                float ring = abs(dist - _GlowCenterRadius);
                float glowAlpha = saturate(1.0 - ring / _GlowRadius);
                glowAlpha = pow(glowAlpha, _GlowIntensity);

                fixed4 col = tex2D(_MainTex, i.uv);

                // Chỉ hiển thị glow khi pixel thực sự nằm ngoài sprite (alpha = 0)
                float mask = step(0.001, col.a); // 0 nếu alpha gần 0, 1 nếu có nội dung
                glowAlpha *= (1.0 - mask); // Loại glow ở vùng sprite (mask = 1 → glowAlpha = 0)

                fixed3 finalRGB = col.rgb + _GlowColor.rgb * glowAlpha;
                float finalA = max(col.a, glowAlpha * _GlowColor.a);

                return fixed4(finalRGB, finalA);
            }
            ENDCG
        }
    }
}