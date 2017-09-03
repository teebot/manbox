--copy product title in translation texts

INSERT INTO Translation (TranslationId, Code) SELECT TitleTrId, NULL From Product
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT TitleTrId, 1, Title From Product
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT TitleTrId, 2, Title From Product

INSERT INTO Translation (TranslationId, Code) SELECT DescriptionTrId, NULL From Product
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT DescriptionTrId, 1, [Description] From Product
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT DescriptionTrId, 2, [Description] From Product

INSERT INTO Translation (TranslationId, Code) SELECT DescriptionDetailTrId, NULL From Product
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT DescriptionDetailTrId, 1, [DescriptionDetail] From Product
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT DescriptionDetailTrId, 2, [DescriptionDetail] From Product

INSERT INTO Translation (TranslationId, Code) SELECT TitleTrId, NULL From Category
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT TitleTrId, 1, Title From Category
INSERT INTO TranslationText (TranslationId, LanguageId, Text) SELECT TitleTrId, 2, Title From Category
