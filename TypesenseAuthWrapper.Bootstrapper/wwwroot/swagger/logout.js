console.log('Logout plugin loaded');

function delay(time) {
    return new Promise(resolve => setTimeout(resolve, time));
}

delay(1000).then(() => {
    let metadataUrl;
    let clientId;
    
    fetch('/api/typesenseauthwrapper/oidc-config')
        .then(response => response.json())
        .then(config => {
            metadataUrl = config.metadataUrl;
            clientId = config.clientId;
        })
        .catch(error => console.error('Error fetching config:', error));

    const authorizeButton = document.querySelector('.btn.authorize');

    const logoutButton = document.createElement('button');
    logoutButton.className = 'btn authorize unlocked';
    logoutButton.innerHTML = `
        <span>Logout from IDP</span>
        <i class="fa fa-sign-out" style="font-size:20px"></i>`;
    
    logoutButton.style = 'margin-left: 10px;';
    authorizeButton.parentNode.insertBefore(logoutButton, authorizeButton.nextSibling);
    
    logoutButton.addEventListener('click', async () => {
        try {
            const response = await fetch(metadataUrl, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            const metadata = await response.json();
            const logoutUrl = metadata.end_session_endpoint;

            const scopesWithoutProfile = metadata.scopes_supported.filter(scope => scope !== 'profile');
            // Get the current URL
            const currentUrl = window.location.href;
            const logoutParams = new URLSearchParams({
                client_id: clientId,
                redirect_uri: currentUrl,
                response_type: 'code',
                scope: scopesWithoutProfile.join(' ')
            });
            window.location.href = `${logoutUrl}?${logoutParams.toString()}`;

        } catch (error) {
            console.error('Error during logout attempt:', error);
        }
    });
});