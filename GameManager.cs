using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask boardLayer;

    [SerializeField]
    private Disc DiscBlackUp;

    [SerializeField]
    private Disc DiscWhiteUp;

    [SerializeField]
    private GameObject hightlightPrefab;

    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();
    private GameState gameState = new GameState();
    private Disc[,] discs = new Disc[8, 8];
    private bool canMove = true;
    private List<GameObject> hightlights = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        discPrefabs[Player.Black] = DiscBlackUp;
        discPrefabs[Player.White] = DiscWhiteUp;

        AddStartDisc();
        ShowLegalMoves();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, boardLayer))
            {
                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPos(impact);
                OnBoardClicked(boardPos);
            }
        }
    }

    private void ShowLegalMoves()
    {
        foreach (Position boardPos in gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
            GameObject hightlight = Instantiate(hightlightPrefab, scenePos, Quaternion.identity);
            hightlights.Add(hightlight);
        }
    }

    private void HideLegalMoves()
    {
        hightlights.ForEach(Destroy);
        hightlights.Clear();
    }

    private void OnBoardClicked(Position boardPos)
    {
        if (!canMove)
        {
            return;
        }
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        canMove = false;
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        ShowLegalMoves();
        canMove = true;
    }

    private Position SceneToBoardPos(Vector3 scenePos)
    {
        int col = (int)(scenePos.x - 0.25f);
        int row = 7 - (int)(scenePos.z - 0.25f);
        return new Position(row, col);
    }

    private Vector3 BoardToScenePos(Position boardPos)
    {
        return new Vector3(boardPos.Col + 0.75f, 0, 7 - boardPos.Row + 0.75f);
    }

    private void SpawnDics(Disc prefab, Position boardPos)
    {
        Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.1f;
        discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenePos, Quaternion.identity);
    }

    private void AddStartDisc()
    {
        foreach (Position boardPos in gameState.OccupiedPosition())
        {
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDics(discPrefabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            discs[boardPos.Row, boardPos.Col].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnDics(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.OutFlanked);
        yield return new WaitForSeconds(0.83f);
    }
}
