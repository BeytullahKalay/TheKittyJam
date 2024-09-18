Shader "Custom/PanelWithCircularHole"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _HoleCenter("Hole Center", Vector) = (0.5, 0.5, 0, 0)
        _HoleRadius("Hole Radius", Float) = 0.25
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _HoleCenter;
            float _HoleRadius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = o.vertex;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV = screenUV * 0.5f + 0.5f;

                float2 holeCenter = _HoleCenter.xy;

                // Ekran oranını hesaba kat
                float aspectRatio = _ScreenParams.x / _ScreenParams.y;
                float2 scaledUV = screenUV - holeCenter;
                scaledUV.x *= aspectRatio;

                float dist = length(scaledUV);

                if (dist < _HoleRadius)
                {
                    discard;
                }

                half4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDHLSL
        }
    }
}
