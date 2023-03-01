public enum Piece : byte
{
    Empty = 0,
    White = 1,
    Black = 2
}

public struct Connect3State
{
    const uint initialVerifier = 0b0011_0011_0011_0011_0011_0011_0011_0011;
    const uint mask1 = 0b0000_0000_0000_0000_0000_0000_0000_1111;
    const uint mask2 = 0b0000_0000_0000_0000_0000_0000_1111_0000;
    const uint mask3 = 0b0000_0000_0000_0000_0000_1111_0000_0000;
    const uint mask4 = 0b0000_0000_0000_0000_1111_0000_0000_0000;
    const uint mask5 = 0b0000_0000_0000_1111_0000_0000_0000_0000;
    const uint mask6 = 0b0000_0000_1111_0000_0000_0000_0000_0000;
    const uint mask7 = 0b0000_1111_0000_0000_0000_0000_0000_0000;
    const uint mask8 = 0b1111_0000_0000_0000_0000_0000_0000_0000;
    const uint white = 0b0110_0110_0110_0110_0110_0110_0110_0110;
    const uint black = 0b0000_0000_0000_0000_0000_0000_0000_0000;

    private readonly ushort levels;
    private readonly uint stateInfo;
    private readonly uint collumnVerifier;
    private readonly uint lineVerifier;
    private readonly uint diagonalVerifier;

    public Piece this[int i, int j]
        => (Piece)((stateInfo >> 2 * i + 8 * j) % 4);

    public Connect3State()
    {
        this.stateInfo = 0;
        this.levels = 0;
        this.collumnVerifier = initialVerifier;
        this.lineVerifier = initialVerifier;
        this.diagonalVerifier = initialVerifier;
    }

    public Connect3State(Connect3State state, byte x, Piece piece)
    {
        var p = (uint)piece;
        var level = state.GetLevel(x);
        var bitPosition = 4 * level + x;
        p <<= (int)(2 * bitPosition);
        
        this.stateInfo = state.stateInfo | p;
        
        this.levels = (ushort)state.addLevel(x);
        this.collumnVerifier = initialVerifier;
        this.lineVerifier = initialVerifier;
        this.diagonalVerifier = initialVerifier;
        
    }
    
    public uint GetLevel(byte collumn)
    {
        uint level = this.levels;
        switch (collumn)
        {
            case 0:
                level &= mask1;
                break;
            case 1:
                level &= mask2;
                level >>= 4;
                break;
            case 2:
                level &= mask3;
                level >>= 8;
                break;
            default:
                level &= mask4;
                level >>= 12;
                break;
        }
        return level;
    }

    public Piece CheckWin()
    {
        if (validateWins(white))
            return Piece.White;
        else if (validateWins(black))
            return Piece.Black;
        return Piece.Empty;
    }

    private bool validateWins(uint mask) =>  
        validateMask(lineVerifier ^ mask) || 
        validateMask(collumnVerifier ^ mask) || 
        validateMask(diagonalVerifier ^ mask);

    private bool validateMask(uint maskedVerifier)
    {
        uint validator = 
            (maskedVerifier & mask1) |
            (maskedVerifier & mask2) |
            (maskedVerifier & mask3) |
            (maskedVerifier & mask4) |
            (maskedVerifier & mask5) |
            (maskedVerifier & mask6) |
            (maskedVerifier & mask7) |
            (maskedVerifier & mask8);
        
        return validator > 0;
    }

    private uint addLevel(byte collumn)
        => this.levels + (1u << (4 * collumn));
}