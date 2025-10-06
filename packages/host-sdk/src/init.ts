/**
 * Initialize shared dependencies for extensions
 * Import this file as a side-effect at the top of your host app
 */

import * as React from "react";
import * as ReactDOM from "react-dom";
import * as ReactRouterDOM from "react-router-dom";
import * as Effector from "effector";
import * as ReactJSXRuntime from "react/jsx-runtime";
import axios from "axios";

// Import effector-react - all exports will use our React instance
import * as EffectorReactModule from "effector-react";

/**
 * Initialize window.__SHARED__ with all dependencies
 * This runs as a side-effect when the module is imported
 */
if (!window.__SHARED__) {
  // Create React with jsx-runtime for proper JSX support
  const ReactWithJSX = Object.assign({}, React, {
    jsxRuntime: ReactJSXRuntime,
  });

  // Use effector-react as-is - it will use the React from this module's scope
  // which is the same React we're exposing to extensions
  const EffectorReact = EffectorReactModule;

  window.__SHARED__ = {
    react: ReactWithJSX,
    reactDOM: ReactDOM,
    reactRouterDOM: ReactRouterDOM,
    effector: Effector,
    effectorReact: EffectorReact,
    axios: axios as any, // Type mismatch between different axios versions
  };

  // Expose React version for extension validation
  window.__SHARED_VERSION__ = React.version;

  console.log(`[host-sdk] ✅ Initialized __SHARED__ v${React.version}`);
}

// Re-export for convenience
export { React, ReactDOM, ReactRouterDOM, Effector, axios };
export { EffectorReactModule as EffectorReact };

