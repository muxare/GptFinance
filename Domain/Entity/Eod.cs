﻿namespace GptFinance.Domain.Entity
{
    public record Eod(DateTime Date, decimal Open, decimal High, decimal Low, decimal Close, decimal AdjClose, int Volume);
}
