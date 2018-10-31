Shader "Custom/PortalWindow" 
{
	SubShader 
	{
		ZWrite off
		ColorMask 0
		Cull Off

		Stencil{
			Ref 1 //将 referenceValue 设置成 1
			Comp always //default value
			Pass replace
		}


		Pass
		{

		}
	}
	FallBack "Diffuse"
}
