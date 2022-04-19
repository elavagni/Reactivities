/// <reference types="cypress" />

describe('Login to the application', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000');
  });

  it('displays logo and header', () => {
    cy.get('h1')
      .should('have.text', 'Reactivities')
      .get('img')
      .should('have.attr', 'src', '/assets/logo.png');
  });

  it('displays login and register dialogs', () => {
    //Check for Login and Register buttons
    cy.get('button').should('have.length', 2);
    cy.contains('Login!');
    cy.contains('Register');
    //Display Login form
    cy.contains('Login!').click();
    cy.get('div.ui.dimmer')
      .contains('Login to Reactivities')
      .get('input[name=email]')
      .get('input[name=password]');
    cy.contains('Login!');
    //Close form
    cy.get('body').click('topLeft');
    cy.get('div.ui.dimmer').should('not.exist');
    //Display Register form
    cy.contains('Register').click();
    cy.get('div.ui.dimmer')
      .contains('Sign up to Reactivities!')
      .get('input[name=displayName]')
      .get('input[name=userName]')
      .get('input[name=email]')
      .get('input[name=password]');
    cy.contains('Register');
    //Close form
    cy.get('body').click('topLeft');
    cy.get('div.ui.dimmer').should('not.exist');
  });
});
