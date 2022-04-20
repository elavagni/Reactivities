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
    cy.get('div.ui.dimmer').contains('Sign up to Reactivities!');
    //Check fields and validation
    cy.get('input[name=displayName]').focus().blur();
    cy.contains('displayName is a required field');
    cy.get('input[name=userName]').focus().blur();
    cy.contains('userName is a required field');
    cy.get('input[name=email]').focus().blur();
    cy.contains('email is a required field');
    cy.get('input[name=password]').focus().blur();
    cy.contains('password is a required field');
    //Contains CTA
    cy.contains('Register');
    //Close form
    cy.get('body').click('topLeft');
    cy.get('div.ui.dimmer').should('not.exist');
  });
});
