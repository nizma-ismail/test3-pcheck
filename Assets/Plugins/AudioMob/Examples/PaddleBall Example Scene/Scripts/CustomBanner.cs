using UnityEngine;
using AudioMob;

namespace AudioMobExamples.PaddleBall
{
	/// <summary>
	/// Custom banner using a 3D banner texture,
	/// </summary>
	public sealed class CustomBanner : Banner
	{
		[SerializeField]
		private SpriteRenderer billboardImage;
		
		[SerializeField]
		private Texture2D defaultBannerTexture;

		private void Awake()
		{
			Debug.Assert(billboardImage, $"{nameof(billboardImage)} reference is null.");
			Debug.Assert(defaultBannerTexture, $"{nameof(defaultBannerTexture)} reference is null.");
		}

		public override void Show(Texture2D bannerTexture)
		{
			SetBanner(bannerTexture);
		}

		public override void Hide()
		{
			SetBanner(defaultBannerTexture);	
			countdown.Hide();
		}

		private void SetBanner(Texture2D texture2D)
		{
			billboardImage.sprite = Sprite.Create(texture2D,
				new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
		}

		public override void ShowSkipButton()
		{
			// Un-needed as paddle ball is always rewarded.
		}
	}

}
