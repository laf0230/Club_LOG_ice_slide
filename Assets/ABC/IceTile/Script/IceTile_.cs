using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceTile_ : MonoBehaviour
{
    public TileData tileData;
    public float offset = 0.5f; // 충돌체와의 거리 오프셋
    public bool isMoveing = false;
    public string word;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = word;
    }

    void Update()
    {
        if (!isMoveing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) StartSlide(Vector2.up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) StartSlide(Vector2.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartSlide(Vector2.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) StartSlide(Vector2.right);
        }
    }

    void StartSlide(Vector2 direction)
    {
        isMoveing = true;
        // Ray 시작 위치 및 방향 설정
        Ray2D ray = new Ray2D(transform.position, direction);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, tileData.targetLayer);

        if (hits.Length > 1) // 자기 자신의 콜라이더를 제외한 충돌 확인
        {
            Vector2 hitPoint = hits[1].point;
            Vector2 adjustedPoint = hitPoint - (direction * offset);

            StartCoroutine(Move(adjustedPoint));
            Debug.DrawLine(transform.position, hitPoint, Color.red, 1f);
        }
        else
        {
            Debug.Log("Ray did not hit any object.");
        }
    }

    IEnumerator Move(Vector2 targetPosition)
    {
        Vector3 startPosition = transform.position; // 시작 위치 설정
        float distance = Vector3.Distance(startPosition, targetPosition); // 시작 위치와 목표 위치 간 거리 계산
        float duration = distance / tileData.speed; // 이동 거리 기반으로 지속 시간 계산

        float elapsedTime = 0f; // 경과 시간 초기화

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // 시간 누적
            yield return null; // 다음 프레임까지 대기
        }

        isMoveing = false;
        transform.position = targetPosition; // 최종 위치 보정
    }

private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.collider.CompareTag("MovableBlock"))
    {
        IceTile_ targetTile = collision.collider.GetComponent<IceTile_>();
        if (targetTile != null)
        {
            string targetWord = targetTile.word;

            // 조합 시도
            string combinedWord = CombineHangul(word, targetWord);
            if (!string.IsNullOrEmpty(combinedWord))
            {
                // 조합 성공 시 현재 타일의 word 갱신
                word = combinedWord;
                text.text = word;

                // 조합된 타일 제거
                Destroy(targetTile.gameObject);
            }
            else
            {
                Debug.Log("조합 불가능: " + word + " + " + targetWord);
            }
        }
    }
}

    private string CombineHangul(string baseWord, string targetWord)
    {
        if (string.IsNullOrEmpty(baseWord) || string.IsNullOrEmpty(targetWord))
            return null;

        char baseChar = baseWord[baseWord.Length - 1];
        char targetChar = targetWord[0];

        // 초성/중성/종성 구분
        int baseCode = baseChar - 0xAC00; // 한글 유니코드 시작점
        int targetCode = targetChar - 0xAC00;

        // 초성/중성/종성 테이블
        string choTable = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        string jungTable = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅛㅜㅠㅡㅣㅘㅙㅚㅝㅞㅟㅢ";
        string jongTable = "ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

        // 현재 문자가 초성인지, 중성인지 확인
        if (choTable.Contains(baseChar) && jungTable.Contains(targetChar))
        {
            // 초성 + 중성 결합
            int choIndex = choTable.IndexOf(baseChar);
            int jungIndex = jungTable.IndexOf(targetChar);

            char combinedChar = (char)(0xAC00 + (choIndex * 21 * 28) + (jungIndex * 28));
            return baseWord.Substring(0, baseWord.Length - 1) + combinedChar;
        }
        else if (baseCode >= 0 && baseCode < 11172) // 유효한 음절인 경우
        {
            int baseCho = baseCode / (21 * 28); // 초성
            int baseJung = (baseCode % (21 * 28)) / 28; // 중성
            int baseJong = baseCode % 28; // 종성

            if (baseJong == 0 && jongTable.Contains(targetChar))
            {
                // 종성이 없는 경우, 종성 추가
                int jongIndex = jongTable.IndexOf(targetChar);
                char combinedChar = (char)(0xAC00 + (baseCho * 21 * 28) + (baseJung * 28) + jongIndex);
                return baseWord.Substring(0, baseWord.Length - 1) + combinedChar;
            }
        }

        return null; // 조합 불가능
    }
}
