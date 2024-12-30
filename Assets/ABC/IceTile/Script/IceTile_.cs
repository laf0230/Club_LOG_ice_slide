using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceTile_ : MonoBehaviour
{
    public TileData tileData;
    public float offset = 0.5f; // 충돌체와의 거리 오프셋
    public float rayOffset = 0.5f; // 충돌체와의 거리 오프셋
    public bool isMoveing = false;
    public string word;

    private TextMeshProUGUI text;
    private Coroutine slideCoroutine; // 실행 중인 코루틴을 추적
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GetComponentInParent<LevelManager>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = word;
    }

    void Update()
    {
        if (!isMoveing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) StartSlideToPhysics(Vector2.up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) StartSlideToPhysics(Vector2.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartSlideToPhysics(Vector2.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) StartSlideToPhysics(Vector2.right);
        }
    }

    void StartSlideToPhysics(Vector2 direction)
    {
        isMoveing = true;
        // 충돌 체크
        if (CheckForCollision(transform.position, direction))
        {
            Debug.Log("Movement stopped due to collision");
            StopSlideCoroutine();
        }
        else
        {
            StartSlideCoroutine(direction);
        }
    }

    bool CheckForCollision(Vector3 position, Vector2 direction)
    {
        Ray2D ray = new Ray2D(position, direction);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, offset, tileData.targetLayer);

        // 충돌이 있는지 확인 (자기 자신 제외)
        return hits.Length > 1;
    }

    void StartSlideCoroutine(Vector2 direction)
    {
        // 기존 코루틴이 있으면 중지
        StopSlideCoroutine();

        // 새 슬라이드 코루틴 시작
        slideCoroutine = StartCoroutine(SlidePhysics(direction));
    }

    void StopSlideCoroutine()
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }
    }

    IEnumerator SlidePhysics(Vector2 direction)
    {
        SoundManager.Instance.PlaySFXMusic("TileMove");
        float stepDistance = 0.1f; // 이동 단위 거리

        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + (Vector3)direction * stepDistance;

            // 목표 지점으로 부드럽게 이동
            yield return SmoothMove(startPosition, targetPosition, tileData.speed, direction);

            // 목표 지점에서 충돌 체크
            transform.position = targetPosition;

            if (CheckForCollision(transform.position, direction))
            {
                Debug.Log("Collision detected, stopping movement");
                isMoveing = false;
                break; // 충돌 시 이동 중단
            }
        }

        slideCoroutine = null; // 코루틴 종료 시 초기화
    }

    IEnumerator SmoothMove(Vector3 start, Vector3 end, float speed, Vector2 direction)
    {
        float elapsedTime = 0f;
        float travelTime = Vector3.Distance(start, end) / speed;

        while (elapsedTime < travelTime)
        {
            if (CheckForCollision(transform.position, direction)) break;

            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsedTime / travelTime);
            yield return null;
        }

        transform.position = end; // 이동 완료 후 정확히 목표 지점에 위치
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
                    SoundManager.Instance.PlaySFXMusic("TileCombine");

                levelManager.OnBlockCrushed(this, combinedWord);

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
        string jungTable = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅛㅜㅠㅡㅣ";
        string jongTable = "ㄱㅅㅇㅈㅊㅋㅌㅍㅎ";

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
