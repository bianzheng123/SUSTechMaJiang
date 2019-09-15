using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAnim : MonoBehaviour {
    [SerializeField]
    private GameObject waitingProfile;

	private void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.NowGameState = GameState.DEAL_CARDS;
        waitingProfile.SetActive(false);
    }
}
