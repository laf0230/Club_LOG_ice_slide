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
    private Coroutine slideCoroutine; // ���� ���� �ڷ�ƾ�� ����
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
        // �浹 üũ
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

        // �浹�� �ִ��� Ȯ�� (�ڱ� �ڽ� ����)
        return hits.Length > 1;
    }

    void StartSlideCoroutine(Vector2 direction)
    {
        // ���� �ڷ�ƾ�� ������ ����
        StopSlideCoroutine();

        // �� �����̵� �ڷ�ƾ ����
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
        float speed = tileData.speed; // �̵� �ӵ�

        // Rigidbody2D�� �����ɴϴ�.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing!");
            yield break;
        }

        // Force�� ������ ���� ����
        Vector2 forceDirection = direction.normalized * speed;

        // �浹 ���θ� �ݺ������� üũ
        while (true)
        {
            // �浹 ���� Ȯ�� (�̵��� �ϱ� �� �浹 üũ)
            if (CheckForCollision(transform.position, forceDirection))
            {
                // �浹�� ������ isMoving�� false�� �����ϰ� �̵��� ����
                Debug.Log("Collision detected, stopping movement");

                // velocity�� 0���� �����Ͽ� ������ �̵��� ����
                rb.velocity = Vector2.zero;

                break; // �浹 �� �̵� �ߴ�
            }

            // �̵��� ���� AddForce ����
            rb.AddForce(forceDirection, ForceMode2D.Force);

            // ���� �ð� �� ���ߵ��� ���� (Ÿ�� ũ�⸸ŭ �̵��ϴ� �Ϳ� ���߾� ����)
            yield return new WaitForSeconds(0.1f); // �ʿ信 ���� ���� ����

            // �̵� �� �浹 �߻� �� �ߴ�
            if (CheckForCollision(transform.position, forceDirection))
            {
                Debug.Log("Collision detected during movement");

                // velocity�� 0���� �����Ͽ� ������ �̵��� ����
                rb.velocity = Vector2.zero;

                yield break; // �̵��� ���߰� �ڷ�ƾ ����
            }
        }

        slideCoroutine = null; // �ڷ�ƾ ���� �� �ʱ�ȭ
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MovableBlock"))
        {
            IceTile_ targetTile = collision.collider.GetComponent<IceTile_>();
            if (targetTile != null)
            {
                string targetWord = targetTile.word;

                // ���� �õ�
                string combinedWord = CombineHangul(word, targetWord);
                if (!string.IsNullOrEmpty(combinedWord))
                {
                    SoundManager.Instance.PlaySFXMusic("TileCombine");

                    levelManager.OnBlockCrushed(this, combinedWord);

                    // ���� ���� �� ���� Ÿ���� word ����
                    word = combinedWord;
                    text.text = word;

                    // ���յ� Ÿ�� ����
                    Destroy(targetTile.gameObject);

                    // Ÿ�� ��ġ�� �̵�
                    targetPosition = targetTile.transform.position;
                    StartCoroutine(MoveToTargetPosition());
                }
                else
                {
                    Debug.Log("���� �Ұ���: " + word + " + " + targetWord);
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

    // Ÿ�� ��ġ�� �̵��ϴ� �ڷ�ƾ
    IEnumerator MoveToTargetPosition()
    {
        float moveSpeed = 5f; // �̵� �ӵ�
        float step = moveSpeed * Time.deltaTime; // �����Ӹ��� �̵��� �Ÿ�

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Ÿ�� ��ġ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            yield return null;
        }

        // ��Ȯ�� ��ġ�� �����ϸ� ����
        transform.position = targetPosition;
    }

    private string CombineHangul(string baseWord, string targetWord)
    {
        if (string.IsNullOrEmpty(baseWord) || string.IsNullOrEmpty(targetWord))
            return null;

        char baseChar = baseWord[baseWord.Length - 1];
        char targetChar = targetWord[0];

        // �ʼ�/�߼�/���� ����
        int baseCode = baseChar - 0xAC00; // �ѱ� �����ڵ� ������
        int targetCode = targetChar - 0xAC00;

        // �ʼ�/�߼�/���� ���̺�
        string choTable = "��������������������������������������";
        string jungTable = "�������¤äĤŤƤǤˤ̤ФѤ�";
        string jongTable = "������������������";

        // ���� ���ڰ� �ʼ�����, �߼����� Ȯ��
        if (choTable.Contains(baseChar) && jungTable.Contains(targetChar))
        {
            // �ʼ� + �߼� ����
            int choIndex = choTable.IndexOf(baseChar);
            int jungIndex = jungTable.IndexOf(targetChar);

            char combinedChar = (char)(0xAC00 + (choIndex * 21 * 28) + (jungIndex * 28));
            return baseWord.Substring(0, baseWord.Length - 1) + combinedChar;
        }
        else if (baseCode >= 0 && baseCode < 11172) // ��ȿ�� ������ ���
        {
            int baseCho = baseCode / (21 * 28); // �ʼ�
            int baseJung = (baseCode % (21 * 28)) / 28; // �߼�
            int baseJong = baseCode % 28; // ����

            if (baseJong == 0 && jongTable.Contains(targetChar))
            {
                // ������ ���� ���, ���� �߰�
                int jongIndex = jongTable.IndexOf(targetChar);
                char combinedChar = (char)(0xAC00 + (baseCho * 21 * 28) + (baseJung * 28) + jongIndex);
                return baseWord.Substring(0, baseWord.Length - 1) + combinedChar;
            }
        }

        return null; // ���� �Ұ���
    }
}
