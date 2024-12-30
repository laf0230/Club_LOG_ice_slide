using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceTile_ : MonoBehaviour
{
    public TileData tileData;
    public bool isMoving = false;
    public string word;
    public Vector3 targetPosition;

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
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) StartSlideToPhysics(Vector2.up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) StartSlideToPhysics(Vector2.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartSlideToPhysics(Vector2.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) StartSlideToPhysics(Vector2.right);
        }
    }

    void StartSlideToPhysics(Vector2 direction)
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        isMoving = true;
        // 충돌 체크
        if (CheckForCollision(transform.position, direction))
        {
            Debug.Log("Movement stopped due to collision");
            StopSlideCoroutine();
        }
        else
        {
            StopAllCoroutines();
            StartSlideCoroutine(direction);
        }
    }

    bool CheckForCollision(Vector3 position, Vector2 direction)
    {
        Ray2D ray = new Ray2D(position, direction);
        Debug.DrawRay(position, direction * tileData.offset, Color.red);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, tileData.rayOffset, tileData.targetLayer);

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
        float speed = tileData.speed; // 이동 속도

        // Rigidbody2D를 가져옵니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing!");
            yield break;
        }

        // Force를 적용할 방향 설정
        Vector2 forceDirection = direction.normalized * speed;

        // 충돌 여부를 반복적으로 체크
        while (true)
        {
            // 충돌 여부 확인 (이동을 하기 전 충돌 체크)
            if (CheckForCollision(transform.position, forceDirection))
            {
                // 충돌이 있으면 isMoving을 false로 설정하고 이동을 멈춤
                Debug.Log("Collision detected, stopping movement");

                // velocity를 0으로 설정하여 물리적 이동을 멈춤
                rb.velocity = Vector2.zero;

                break; // 충돌 시 이동 중단
            }

            // 이동을 위한 AddForce 적용
            rb.AddForce(forceDirection, ForceMode2D.Force);

            // 일정 시간 후 멈추도록 조절 (타일 크기만큼 이동하는 것에 맞추어 설정)
            yield return new WaitForSeconds(0.1f); // 필요에 따라 조정 가능

            // 이동 후 충돌 발생 시 중단
            if (CheckForCollision(transform.position, forceDirection))
            {
                Debug.Log("Collision detected during movement");

                // velocity를 0으로 설정하여 물리적 이동을 멈춤
                rb.velocity = Vector2.zero;

                yield break; // 이동을 멈추고 코루틴 종료
            }
        }

        slideCoroutine = null; // 코루틴 종료 시 초기화
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

                    // 타겟 위치로 이동
                    targetPosition = targetTile.transform.position;
                    StartCoroutine(MoveToTargetPosition());
                }
                else
                {
                    Debug.Log("조합 불가능: " + word + " + " + targetWord);
                }

        isMoving = false;
            }
        }
        else if (collision.collider.CompareTag("OnField"))
        {
            isMoving = false;
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        StopAllCoroutines();
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
    }

    // 타겟 위치로 이동하는 코루틴
    IEnumerator MoveToTargetPosition()
    {
        float moveSpeed = 5f; // 이동 속도
        float step = moveSpeed * Time.deltaTime; // 프레임마다 이동할 거리

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // 타겟 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            yield return null;
        }

        // 정확한 위치에 도달하면 종료
        transform.position = targetPosition;
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
