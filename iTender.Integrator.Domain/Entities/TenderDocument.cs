using iTender.Integrator.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class TenderDocument : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string? DocumentType { get; private set; }

        public string? Title { get; private set; }

        public string? Description { get; private set; }

        public string Url { get; private set; } = default!;

        public DateTime? DatePublished { get; private set; }

        public DateTime? DateModified { get; private set; }

        public string? Format { get; private set; }

        public string? Language { get; private set; }

        private TenderDocument()
        {
        }

        public static TenderDocument Create(
            string externalId,
            string url,
            string? documentType,
            string? title,
            string? description,
            DateTime? datePublished,
            DateTime? dateModified,
            string? format,
            string? language)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Document id is required.", nameof(externalId));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Document url is required.", nameof(url));

            return new TenderDocument
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Url = url,
                DocumentType = documentType,
                Title = title,
                Description = description,
                DatePublished = datePublished,
                DateModified = dateModified,
                Format = format,
                Language = language
            };
        }
    }
}
