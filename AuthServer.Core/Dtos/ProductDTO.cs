﻿namespace AuthServer.Core.Dtos;

public class ProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Decimal Price { get; set; }
    public int Stock { get; set; }
    public string UserId { get; set; }
}