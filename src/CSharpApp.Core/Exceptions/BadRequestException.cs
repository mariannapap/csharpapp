﻿namespace CSharpApp.Core.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}