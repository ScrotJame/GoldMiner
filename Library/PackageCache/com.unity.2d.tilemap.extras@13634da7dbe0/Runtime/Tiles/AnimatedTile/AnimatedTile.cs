using System;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    ///     Animated Tiles are tiles which run through and display a list of sprites in sequence.
    /// </summary>
    [Serializable]
    [HelpURL(
        "https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/AnimatedTile.html")]
    public class AnimatedTile : TileBase
    {
        /// <summary>
        ///     The List of Sprites set for the Animated Tile.
        ///     This will be played in sequence.
        /// </summary>
        public Sprite[] m_AnimatedSprites;

        /// <summary>
        ///     The minimum possible speed at which the Animation of the Tile will be played.
        ///     A speed value will be randomly chosen between the minimum and maximum speed.
        /// </summary>
        public float m_MinSpeed = 1f;

        /// <summary>
        ///     The maximum possible speed at which the Animation of the Tile will be played.
        ///     A speed value will be randomly chosen between the minimum and maximum speed.
        /// </summary>
        public float m_MaxSpeed = 1f;

        /// <summary>
        ///     The starting time of this Animated Tile.
        ///     This allows you to start the Animation from time in the list of Animated Sprites depending on the
        ///     Tilemap's Animation Frame Rate.
        /// </summary>
        public float m_AnimationStartTime;

        /// <summary>
        ///     The starting frame of this Animated Tile.
        ///     This allows you to start the Animation from a particular Sprite in the list of Animated Sprites.
        ///     If this is set, this overrides m_AnimationStartTime.
        /// </summary>
        public int m_AnimationStartFrame;

        /// <summary>
        ///     The Collider Shape generated by the Tile.
        /// </summary>
        public Tile.ColliderType m_TileColliderType;

        /// <summary>
        ///     Flags for controlling the Tile Animation.
        /// </summary>
        public TileAnimationFlags m_TileAnimationFlags;

        /// <summary>
        ///     Retrieves any tile rendering data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            if (m_AnimatedSprites != null && m_AnimatedSprites.Length > 0)
            {
                tileData.sprite = m_AnimatedSprites[m_AnimatedSprites.Length - 1];
                tileData.colliderType = m_TileColliderType;
            }
        }

        /// <summary>
        ///     Retrieves any tile animation data from the scripted tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileAnimationData">Data to run an animation on the tile.</param>
        /// <returns>Whether the call was successful.</returns>
        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap,
            ref TileAnimationData tileAnimationData)
        {
            if (m_AnimatedSprites.Length > 0)
            {
                tileAnimationData.animatedSprites = m_AnimatedSprites;
                tileAnimationData.animationSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);
                tileAnimationData.animationStartTime = m_AnimationStartTime;
                tileAnimationData.flags = m_TileAnimationFlags;
                if (0 < m_AnimationStartFrame && m_AnimationStartFrame <= m_AnimatedSprites.Length)
                {
                    var tilemapComponent = tilemap.GetComponent<Tilemap>();
                    if (tilemapComponent != null && tilemapComponent.animationFrameRate > 0)
                        tileAnimationData.animationStartTime =
                            (m_AnimationStartFrame - 1) / tilemapComponent.animationFrameRate;
                }

                return true;
            }

            return false;
        }
    }
}