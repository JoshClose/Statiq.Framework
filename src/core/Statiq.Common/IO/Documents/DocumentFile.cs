﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Statiq.Common
{
    internal class DocumentFile : IFile
    {
        private readonly DocumentFileProvider _fileProvider;
        private readonly IDocument _document;

        internal DocumentFile(DocumentFileProvider fileProvider, in NormalizedPath path)
        {
            _fileProvider = fileProvider.ThrowIfNull(nameof(fileProvider));
            path.ThrowIfNull(nameof(path));
            if (!fileProvider.Files.TryGetValue(path, out _document))
            {
                _document = null;
            }
            Path = path;
        }

        public NormalizedPath Path { get; }

        NormalizedPath IFileSystemEntry.Path => Path;

        public IContentProvider GetContentProvider() =>
            _document?.ContentProvider ?? new NullContent();

        public IContentProvider GetContentProvider(string mediaType) =>
            _document?.ContentProvider.CloneWithMediaType(mediaType) ?? new NullContent(mediaType);

        public IDirectory Directory => new DocumentDirectory(_fileProvider, Path.Parent);

        public bool Exists => _document is object;

        public Stream OpenRead() =>
            _document is null ? throw new FileNotFoundException() : _document.GetContentStream();

        public TextReader OpenText() =>
            _document is null ? throw new FileNotFoundException() : _document.GetContentTextReader();

        public async Task<string> ReadAllTextAsync(CancellationToken cancellationToken = default) =>
            _document is null ? throw new FileNotFoundException() : await _document.GetContentStringAsync();

        public long Length => _document?.ContentProvider.GetLength() ?? throw new FileNotFoundException();

        public string MediaType => _document?.ContentProvider.MediaType ?? throw new FileNotFoundException();

        public DateTime LastWriteTime => throw new NotSupportedException();

        public DateTime CreationTime => throw new NotSupportedException();

        public Task CopyToAsync(IFile destination, bool overwrite = true, bool createDirectory = true, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task MoveToAsync(IFile destination, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public void Delete() => throw new NotSupportedException();

        public Stream OpenAppend(bool createDirectory = true) => throw new NotSupportedException();

        public Stream Open(bool createDirectory = true) => throw new NotSupportedException();

        public Stream OpenWrite(bool createDirectory = true) => throw new NotSupportedException();

        public Task WriteAllTextAsync(string contents, bool createDirectory = true, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override string ToString() => Path.ToString();

        public string ToDisplayString() => Path.ToDisplayString();
    }
}
