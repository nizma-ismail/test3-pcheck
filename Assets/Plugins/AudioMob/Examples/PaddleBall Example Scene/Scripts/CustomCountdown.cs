using UnityEngine;
using UnityEngine.UI;

namespace AudioMobExamples.PaddleBall
{
	/// <summary>
	/// An example of a custom Countdown for the paddle ball example game.
	/// </summary>
	public sealed class CustomCountdown : AudioMob.Countdown
	{
		[SerializeField]
		[Tooltip("The countdown displays how much time left in an ad")]
		private Text adCountdownText;

		private void Awake()
		{
			Debug.Assert(adCountdownText, $"{nameof(adCountdownText)} reference is null.");
			adCountdownText.text = "--:--";
		}

		public override void Show()
		{
			adCountdownText.text = "00:00";
		}

		public override void Hide()
		{
			adCountdownText.text = "--:--";
		}

		public override void UpdateCountdown(float timeRemaining, float percentageRemaining)
		{
			string timeLeft = "" + Mathf.Round(timeRemaining);
			if (timeLeft.Length == 1)
			{
				timeLeft = "0" + timeLeft;
			}

			adCountdownText.text = "00:" + timeLeft;
		}
	}

}
