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

  it('displays login and register buttons', () => {
    cy.get('button').should('have.length', 2);
    cy.contains('Login!');
    cy.contains('Register');
  });
});
