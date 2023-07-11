Shader "Hidden/AsciiPostProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_FontTex ("Font", 2D) = "white" {}
		_CharCount("Character Count", int) = 256
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _FontTex;
			float4 _MainTex_TexelSize;
			int _CharCount;

			fixed4 frag(v2f i) : SV_Target
			{
				//return tex2D(_FontTex, i.uv);

				fixed4 col = tex2D(_MainTex, i.uv);
				//return col;

				float2 uv = (i.uv * _MainTex_TexelSize.zw) % 1;
				//return float4(uv, 0, 1);

				float darkness = 1 - (col.r + col.g + col.b) / 3.0;
				float2 fontUv = float2(uv.x / _CharCount, uv.y);
				fontUv.x += floor(darkness * (_CharCount - 1)) / _CharCount;
				//return float4(fontUv, 0, 1);

				float fontLightness = tex2D(_FontTex, fontUv).r;
				fixed4 fontColor = fixed4(fontLightness, fontLightness, fontLightness, 1);


				float lightness = (col.r + col.g + col.b) / 3.0;

				if (col.r < lightness * 0.85) {
					fontColor.r = 0;
				}

				if (col.g < lightness * 0.85) {
					fontColor.g = 0;
				}
                
				if (col.b < lightness * 0.85) {
					fontColor.b = 0;
				}

                return fontColor;
            }
            ENDCG
        }
    }
}
