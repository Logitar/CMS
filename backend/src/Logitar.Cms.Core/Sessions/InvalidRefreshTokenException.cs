﻿using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Sessions;

public class InvalidRefreshTokenException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified refresh token is not valid.";

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public InvalidRefreshTokenException(string refreshToken, string propertyName, Exception? innerException = null)
    : base(BuildMessage(refreshToken, propertyName), innerException)
  {
    RefreshToken = refreshToken;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string refreshToken, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RefreshToken), refreshToken)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
