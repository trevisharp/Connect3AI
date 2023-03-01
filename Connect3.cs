using System;

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

    public Piece this[int i, int j] => (Piece)((stateInfo >> 2 * i + 8 * j) % 4);
    
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
        if (level > 3)
            throw new Exception("Column is fully");

        var bitPosition = 4 * level + x;
        p <<= (int)(2 * bitPosition);
        
        this.stateInfo = state.stateInfo | p;
        
        this.levels = (ushort)state.addLevel(x);

        this.collumnVerifier = initialVerifier;
        this.lineVerifier = initialVerifier;
        this.diagonalVerifier = initialVerifier;
        
        this.collumnVerifier = updateVerifier(
            state.collumnVerifier, level, x, (uint)piece
        );

        this.lineVerifier = updateVerifier(
            state.lineVerifier, x, level, (uint)piece
        );

        this.diagonalVerifier = updateDiagonalVerifier(
            state.diagonalVerifier, x, level, (uint)piece
        );
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

    private bool validateMask(uint maskedVerifier) =>
            (maskedVerifier & mask1) == 0 ||
            (maskedVerifier & mask2) == 0 ||
            (maskedVerifier & mask3) == 0 ||
            (maskedVerifier & mask4) == 0 ||
            (maskedVerifier & mask5) == 0 ||
            (maskedVerifier & mask6) == 0 ||
            (maskedVerifier & mask7) == 0 ||
            (maskedVerifier & mask8) == 0;
    
    private uint addLevel(byte collumn)
        => this.levels + (1u << (4 * collumn));

    private uint updateVerifier(uint verifier, uint line, uint column, uint piece)
    {
        var pieceValue = 3 - 2 * (int)piece;
        int bitSelector = (int)(8 * line);

        if (column < 3)
            verifier = updateVerifier(verifier, bitSelector, pieceValue);
        
        if (column > 0)
            verifier = updateVerifier(verifier, bitSelector + 4, pieceValue);
        
        return verifier;
    }

    private uint updateDiagonalVerifier(uint verifier, uint line, uint column, uint piece)
    {
        var pieceValue = 3 - 2 * (int)piece;

        if (line == column)
        {
            if (line < 3)
                verifier = updateVerifier(verifier, 0, pieceValue);
            if (line > 0)
                verifier = updateVerifier(verifier, 4, pieceValue);
        }
        else if (line + column == 3)
        {
            if (line < 3)
                verifier = updateVerifier(verifier, 8, pieceValue);
            if (line > 0)
                verifier = updateVerifier(verifier, 12, pieceValue);
        }

        if (line + column == 2)
            verifier = updateVerifier(verifier, 16, pieceValue);
        else if (line + column == 4)
            verifier = updateVerifier(verifier, 20, pieceValue);
        else if (line - column == 1)
            verifier = updateVerifier(verifier, 24, pieceValue);
        else if (line - column == uint.MaxValue)
            verifier = updateVerifier(verifier, 28, pieceValue);

        return verifier;
    }

    private uint updateVerifier(uint verifier, int bitSelector, int pieceValue)
    {
        uint mask = 15u << bitSelector;
        uint value = (verifier & mask) >> bitSelector;
        value = (uint)(value + pieceValue);
        verifier = verifier & ~mask | value << bitSelector;
        return verifier;
    }
}
