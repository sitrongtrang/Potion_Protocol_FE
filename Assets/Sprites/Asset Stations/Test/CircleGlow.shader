Shader "Custom/CircleGlow"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _GlowColor("Glow Color", Color) = (1, 0.8, 0.3, 1)
        _GlowIntensity("Glow Intensity", Float) = 1.0
        _GlowRange("Glow Range", Range(0.01, 1)) = 0.2
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);
                float dist = distance(uv, center);

                fixed4 texCol = tex2D(_MainTex, uv);

                float glow = smoothstep(0.5, 0.5 - _GlowRange, dist);
                float alpha = texCol.a + glow * _GlowIntensity;

                float3 finalColor = lerp(texCol.rgb, _GlowColor.rgb, glow * _GlowIntensity);

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
