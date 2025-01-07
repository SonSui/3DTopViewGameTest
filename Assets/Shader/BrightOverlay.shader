Shader "Custom/BrightOverlay"
{
    Properties
    {
<<<<<<< HEAD
        _Brightness ("Brightness", Range(0, 2)) = 1.2 
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
=======
        _OverlayColor ("Overlay Color", Color) = (1, 1, 1, 0.5)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
>>>>>>> origin/main
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

<<<<<<< HEAD
            float _Brightness;
=======
            float4 _OverlayColor;
>>>>>>> origin/main

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
<<<<<<< HEAD
                
                fixed4 baseColor = fixed4(1, 1, 1, 1); 

                
                fixed4 result = baseColor * _Brightness;

                
                return saturate(result);
=======
                return _OverlayColor;
>>>>>>> origin/main
            }
            ENDCG
        }
    }
}
