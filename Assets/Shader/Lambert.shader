Shader "Custom/Lambert"
{
    Properties
    {
        _MainColor("Main Color",Color) = (1,1,1,1)
        _SubColor("Sub Color",Color) = (0.4,0.4,0.4,0.4)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            # pragma vertex vert 
            # pragma fragment frag
            # include "UnityCG.cginc"
            # include "UnityLightingCommon.cginc"

            struct v2f 
            {
                float4 pos : SV_POSITION;
                fixed4 dif : COLOR0;
            };

            fixed4 _MainColor;
            fixed4 _SubColor;
            fixed3 l;
            v2f o;
            v2f vert(appdata_base v)
            {
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 n = UnityObjectToWorldNormal(v.normal);
                n = normalize(n);
                l = normalize(_WorldSpaceLightPos0.xyz);

                fixed ndotl = dot(n,l);
                o.dif = _LightColor0 * _MainColor * saturate(ndotl);
                o.dif = o.dif > 0.3? o.dif : _SubColor; 
                return o;
            }

        fixed4 frag(v2f i): SV_TARGET
        {
            return i.dif;
        }
        ENDCG
        }
    }

}
