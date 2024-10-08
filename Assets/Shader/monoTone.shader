Shader "Custom/monoTone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrayScaleAmount ("Gray Scale Amount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
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
            float _GrayScaleAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11));
                col.rgb = lerp(col.rgb, float3(gray, gray, gray), _GrayScaleAmount);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}