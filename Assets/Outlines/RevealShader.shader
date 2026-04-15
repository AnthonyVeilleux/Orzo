Shader "Custom/RevealShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _RevealLightPos ("Reveal Light Position", Vector) = (0,0,0,0)
        _RevealRadius ("Reveal Radius", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _RevealLightPos;
            float _RevealRadius;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // convert vertex to world space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _RevealLightPos.xyz);

                if (dist > _RevealRadius)
                    discard;

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}