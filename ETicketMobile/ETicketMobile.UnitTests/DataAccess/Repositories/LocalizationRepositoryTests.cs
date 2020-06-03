﻿using System;
using System.Threading.Tasks;
using ETicketMobile.Data.Entities;
using ETicketMobile.DataAccess.Interfaces;
using ETicketMobile.DataAccess.Repositories;
using Moq;
using Xunit;

namespace ETicketMobile.UnitTests.DataAccess.Repositories
{
    public class LocalizationRepositoryTests
    {
        #region Fields

        private readonly Mock<ISettingsRepository> settingsRepositoryMock;
        private readonly Localization localization;

        private readonly string culture;

        #endregion

        public LocalizationRepositoryTests()
        {
            settingsRepositoryMock = new Mock<ISettingsRepository>();

            localization = new Localization { Culture = "ru-RU" };

            culture = "{\"Culture\":\"ru-RU\"}";
        }

        [Fact]
        public void Ctor()
        {
            // Act
            var exception = Record.Exception(() => new LocalizationRepository());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void CtorWithParameters_Positive()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new LocalizationRepository(null));
        }

        [Fact]
        public void CtorWithParameters_Negative()
        {
            // Arrange
            var settingsRepository = new SettingsRepository();

            // Act
            var localizationRepository = new LocalizationRepository(settingsRepository);

            // Assert
            Assert.IsNotType<ArgumentNullException>(localizationRepository);
        }

        [Fact]
        public async Task GetLocalizationAsync()
        {
            // Arrange
            settingsRepositoryMock
                    .Setup(sr => sr.GetByNameAsync(It.IsAny<string>()))
                    .ReturnsAsync(culture);

            var localizationRepository = new LocalizationRepository(settingsRepositoryMock.Object);

            // Act
            var actualLocalization = await localizationRepository.GetLocalizationAsync();

            // Assert
            Assert.Equal(localization.Culture, actualLocalization.Culture);
        }

        [Fact]
        public async Task GetLocalizationAsync_LocalizationShouldBeNull()
        {
            // Arrange
            settingsRepositoryMock
                    .Setup(sr => sr.GetByNameAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => null);

            var localizationRepository = new LocalizationRepository(settingsRepositoryMock.Object);

            // Act
            var actualLocalization = await localizationRepository.GetLocalizationAsync();

            // Assert
            Assert.Null(actualLocalization);
        }

        [Fact]
        public async Task SaveLocalizationAsync()
        {
            // Arrange
            settingsRepositoryMock.Setup(sr => sr.SaveAsync(It.IsAny<string>(), It.IsAny<string>()));

            var localizationRepository = new LocalizationRepository(settingsRepositoryMock.Object);

            // Act
            await localizationRepository.SaveLocalizationAsync(localization);

            // Assert
            settingsRepositoryMock.Verify();
        }
    }
}