using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FixingIt.RoomObjects.SO
{
    //[CreateAssetMenu()]
    public class ToolRecipeManagerSO : ScriptableObject
    {
        [SerializeField] private ToolRecipeSO[] _recipes;
        public ToolRecipeSO[] Recipes => _recipes;

        // Think how to refactor
        // In the worst case is n^3
        public RoomObjectSO TryRecipe(List<RoomObjectSO> pieces)
        {
            foreach (ToolRecipeSO toolRecipeSO in _recipes)
            {

                if (toolRecipeSO.Pieces.Length != pieces.Count)
                    continue;

                // check recipe
                bool[] piecesUsed = new bool[pieces.Count];
                foreach (RoomObjectSO pieceInRecipeSO in toolRecipeSO.Pieces)
                {

                    for (int i = 0; i < pieces.Count; i++)
                    {
                        if (piecesUsed[i])
                            continue;

                        // compare to next pieceInRecipeSO
                        if (pieceInRecipeSO == pieces[i])
                        {
                            piecesUsed[i] = true;
                            break;
                        }
                    }
                }

                // return?
                if (piecesUsed.All(valor => valor))
                    return toolRecipeSO.Output;
            }

            return null;
        }
    }
}
