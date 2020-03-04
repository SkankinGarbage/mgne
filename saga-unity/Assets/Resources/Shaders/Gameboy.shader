Shader "SaGa/Gameboy"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _FadeBorderSize("Fade border size", Range(0, 0.5)) = 0
        _Black("Black in", Color) = (0,0,0,1)
        _DGray("Dark Gray in", Color) = (.226,.226,.226,1)
        _LGray("Light Gray in", Color) = (.668,.668,.668,1)
        _White("White in", Color) = (1,1,1,1)
        _BlackOut("Black out", Color) = (0,0,0,1)
        _DGrayOut("Dark Gray out", Color) = (.226,.226,.226,1)
        _LGrayOut("Light Gray out", Color) = (.668,.668,.668,1)
        _WhiteOut("White out", Color) = (1,1,1,1)
        _FadeOffset("Fade offset", Range(0,1)) = 0.0
		_Invert("Fade Invert", Range(0, 1)) = 0.0
        _FadeColorMod("FadeColorMod", Range(-1, 1)) = -1.0
	}
	SubShader
	{
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
    
		Cull Off 
        ZTest Always
        ZWrite Off
        Blend One OneMinusSrcAlpha

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
                float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
                float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
            sampler2D _MaskTexture;
            fixed4 _Black;
            fixed4 _DGray;
            fixed4 _LGray;
            fixed4 _White;
            fixed4 _BlackOut;
            fixed4 _DGrayOut;
            fixed4 _LGrayOut;
            fixed4 _WhiteOut;
			float _FadeOffset;
			float _SoftFudge;
            float _FadeColorMod;
            float _FadeBorderSize;
			int _Invert;
			int _FlipX;
			int _FlipY;

            float color_d(fixed4 c1, fixed4 c2) {
                return abs(c1[0] - c2[0]) + abs(c1[1] - c2[1]) + abs(c1[2] - c2[2]);
            }

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 current = tex2D(_MainTex, i.uv);
                current.rgb *= current.a;
                
				float weight = _FadeOffset;
                if (_Invert) weight = 1.0 - weight;
                
                if (i.color.a == 0.0) return float4(1, 0, 0, 0);

                float dBlack = color_d(current, _Black);
                float dDgray = color_d(current, _DGray);
                float dLgray = color_d(current, _LGray);
                float dWhite = color_d(current, _White);
                
                int darkness = 0;
                if (current.a > 0.0) {
                    if (dBlack <= dDgray && dBlack <= dLgray && dBlack <= dWhite) {
                        darkness = 0;
                    }
                    if (dDgray <= dBlack && dDgray <= dLgray && dDgray <= dWhite) {
                        darkness = 1;
                    }
                    if (dLgray <= dBlack && dLgray <= dDgray && dLgray <= dWhite) {
                        darkness = 2;
                    }
                    if (dWhite <= dBlack && dWhite <= dDgray && dWhite <= dLgray) {
                        darkness = 3;
                    }
                    
                    if (weight > 0.25) darkness += _FadeColorMod;
                    if (weight > 0.50) darkness += _FadeColorMod;
                    if (weight > 0.75) darkness += _FadeColorMod;
                    
                    float y = i.vertex.y / 480.0;
                    if (y <       _FadeBorderSize / 3)        darkness += 1;
                    if (y <       _FadeBorderSize * 2 / 3)    darkness += 1;
                    if (y <       _FadeBorderSize)            darkness += 1;
                    if (y > 1.0 - (_FadeBorderSize / 3))      darkness += 1;
                    if (y > 1.0 - (_FadeBorderSize * 2 / 3))  darkness += 1;
                    if (y > 1.0 - (_FadeBorderSize))          darkness += 1;
                    
                    if (darkness < 0) darkness = 0;
                    if (darkness > 3) darkness = 3;
                    
                    if (darkness == 0) {
                        return _BlackOut;
                    }
                    if (darkness == 1) {
                        return _DGrayOut;
                    }
                    if (darkness == 2) {
                        return _LGrayOut;
                    }
                    if (darkness == 3) {
                        return _WhiteOut;
                    }
                }
				return current;
			}
			ENDCG
		}
	}
}
