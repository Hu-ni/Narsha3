Shader "EffectMat/ParticleUnlit"
{
    Properties
    {
		_BaseMap("BaseMap", 2D) = "white" {}
		_BaseMapColor("BaseMap Color", Color) = (1,1,1,1)
		[HDR]_EmissionMap("Emission", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
				float4 color: COLOR;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float4 color: COLOR;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			sampler2D _BaseMap;
			float4 _BaseMap_ST;
			float4 _BaseMapColor; 
			float4 _EmissionMap;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_BaseMap, i.uv) * i.color * _BaseMapColor * _EmissionMap;
                return col;
            }
            ENDCG
        }
    }
}
