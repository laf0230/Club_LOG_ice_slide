using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform keywordContainer;
    public string missionWord;
    public float offset = 0.6f;
    public MissionWord keywordPrefab;

    private string[] words => missionWord?.Select(c => c.ToString()).ToArray() ?? new string[0];
    private List<MissionWord> missionWords = new List<MissionWord>();

    public void OnStartLevel()
    {
        if (words == null || words.Length == 0)
        {
            Debug.LogError("단어 배열이 비어 있습니다.");
            return;
        }

        float centerOffset = (words.Length - 1) * offset / 2f;

        // 키워드 생성
        for (int i = 0; i < words.Length; i++)
        {
            MissionWord keywordObject = Instantiate(keywordPrefab, keywordContainer);

            keywordObject.text.text = words[i];

            Vector3 keywordPosition = new Vector3(i * offset - centerOffset, keywordContainer.transform.position.y, 0);

            keywordObject.transform.position = keywordPosition;

            missionWords.Add(keywordObject);
        }
    }

    public bool IsMatchedMissionWord(string word)
    {
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == word)
            {
                return true;
            }
        }
        return false;
    }

    public void OnBlockCrushed(IceTile_ tile, string word)
    {
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == word)
            {
                // Sound
                SoundManager.Instance.PlaySFXMusic("TileCombined");

                // 게임 종료 로직 작성
                tile.gameObject.SetActive(false);
                SetActiveMissionWord(word, false);

                if (IsGameClear())
                {
                    // 클리어 로직 작성
                    UIManager.instance.GetUI(typeof(ClearUI)).gameObject.SetActive(true);
                    SoundManager.Instance.PlaySFXMusic("GameClear");
                }

                return;
            }
        }
    }

    public void SetActiveMissionWord(string word, bool isActive)
    {
        foreach (var item in missionWords)
        {
            if (item.word == word)
            {
                item.gameObject.SetActive(isActive);
            }
        }
    }

    public bool IsGameClear()
    {
         return !missionWords.Any(c => c.gameObject.activeSelf);
    }

    public void ClearLevel()
    {
        UI clearUI = UIManager.instance.GetUI(typeof(ClearUI));

        clearUI.gameObject.SetActive(true);
    }
}
