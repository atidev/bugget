/**
 * Extensions support - only included in builds with VITE_APP_EXTENSIONS
 * This file exports main app's libraries to window for SDK to use
 */

import * as React from "react";
import * as ReactDOM from "react-dom";
import * as ReactRouterDOM from "react-router-dom";
import * as Effector from "effector";
import * as ReactJSXRuntime from "react/jsx-runtime";
import axios from "axios";
import * as EffectorReactModule from "effector-react";

const ReactWithJSX = Object.assign({}, React, {
  jsxRuntime: ReactJSXRuntime,
});

(window as any).__MAIN_APP_LIBS__ = {
  react: ReactWithJSX,
  reactDOM: ReactDOM,
  reactRouterDOM: ReactRouterDOM,
  effector: Effector,
  effectorReact: EffectorReactModule,
  axios: axios,
};

console.log("[extensions-support] ✅ Exported main app libraries");
