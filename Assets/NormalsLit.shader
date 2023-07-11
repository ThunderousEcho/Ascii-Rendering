Shader "Unlit/NormalsLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		//todo: deprecate this shit

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				half3 worldNormal : NORMAL;
				float3 viewT : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				float3 viewT : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.worldNormal);
				o.viewT = normalize(WorldSpaceViewDir(v.vertex));//ObjSpaceViewDir is similar, but localspace.
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                //return col;

				float3 forward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));

				fixed4 c = 1;
				c.rgb = acos(dot(i.worldNormal, forward) / (length(i.worldNormal) * length(forward))) / 3.1416926;
				return c * tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
