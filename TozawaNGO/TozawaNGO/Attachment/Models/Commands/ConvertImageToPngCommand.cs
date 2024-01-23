﻿using MediatR;
using Microsoft.AspNetCore.Http;

namespace TozawaNGO.Attachment.Models.Commands
{
    public class ConvertImageToPngCommand(IFormFile file) : IRequest< byte[]>
    {
        public IFormFile File { get; set; } = file;
    }
}