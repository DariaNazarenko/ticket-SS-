﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Util;
using ETicketMobile.Resources;
using ETicketMobile.Views.ForgotPassword;
using ETicketMobile.Views.Login;
using ETicketMobile.WebAccess.DTO;
using ETicketMobile.WebAccess.Network.Endpoints;
using ETicketMobile.WebAccess.Network.WebServices.Interfaces;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace ETicketMobile.ViewModels.ForgotPassword
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigationService navigationService;
        private readonly IPageDialogService dialogService;

        private readonly IHttpService httpService;

        private ICommand navigateToConfirmForgotPasswordView;
        private ICommand cancelCommand;

        private string emailWarning;

        private const int EmailMaxLength = 50;

        #endregion

        #region Properties

        public ICommand NavigateToConfirmForgotPasswordView => navigateToConfirmForgotPasswordView 
            ??= new Command<string>(OnNavigateToConfirmForgotPasswordView);

        public ICommand CancelCommand => cancelCommand 
            ??= new Command(OnCancelCommand);

        public string EmailWarning
        {
            get => emailWarning;
            set => SetProperty(ref emailWarning, value);
        }

        #endregion

        public ForgotPasswordViewModel(
            INavigationService navigationService,
            IPageDialogService dialogService,
            IHttpService httpService
        ) : base(navigationService)
        {
            this.navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            this.dialogService = dialogService
                ?? throw new ArgumentNullException(nameof(dialogService));

            this.httpService = httpService
                ?? throw new ArgumentNullException(nameof(httpService));
        }

        private async void OnNavigateToConfirmForgotPasswordView(string email)
        {
            await NavigateToConfirmForgotPasswordViewAsync(email);
        }

        private async Task NavigateToConfirmForgotPasswordViewAsync(string email)
        {
            try
            {
                if (!await IsValidAsync(email))
                    return;

                await RequestActivationCodeAsync(email);
            }
            catch (WebException)
            {
                await dialogService.DisplayAlertAsync("Error", "Check connection with server", "OK");

                return;
            }

            var navigationParameters = new NavigationParameters { { "email", email } };
            await navigationService.NavigateAsync(nameof(ConfirmForgotPasswordView), navigationParameters);
        }

        private async void OnCancelCommand()
        {
            await navigationService.NavigateAsync(nameof(LoginView));
        }

        #region Validation

        private async Task<bool> IsValidAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                //TODO incorrect
                EmailWarning = AppResource.EmailCorrect;

                return false;
            }

            if (!IsEmailValid(email))
            {
                EmailWarning = AppResource.EmailInvalid;

                return false;
            }

            // TODO EmailHasCorrectLength
            if (!IsEmailConstainsCorrectLong(email))
            {
                EmailWarning = AppResource.EmailCorrectLong;

                return false;
            }

            var userExists = await RequestUserExistsAsync(email);

            if (!userExists)
            {
                EmailWarning = AppResource.EmailWrong;

                return false;
            }

            return true;
        }

        private async Task<bool> RequestUserExistsAsync(string email)
        {
            var signUpRequestDto = new ForgotPasswordRequestDto { Email = email };

            var isUserExists = await httpService.PostAsync<ForgotPasswordRequestDto, ForgotPasswordResponseDto>(
                    AuthorizeEndpoint.CheckEmail, 
                    signUpRequestDto);

            return isUserExists.Succeeded;
        }

        private bool IsEmailValid(string email)
        {
            return Patterns.EmailAddress.Matcher(email).Matches();
        }

        private bool IsEmailConstainsCorrectLong(string email)
        {
            return email.Length <= EmailMaxLength;
        }

        #endregion

        private async Task RequestActivationCodeAsync(string email)
        {
            await httpService.PostAsync<string, string>(AuthorizeEndpoint.RequestActivationCode, email);
        }
    }
}