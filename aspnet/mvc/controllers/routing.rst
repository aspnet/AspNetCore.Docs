
Routing to Controller Actions
========================================

By `Ryan Nowak`_ and `Rick Anderson`_

- fixes https://github.com/aspnet/Docs/issues/1795

ASP.NET Core MVC uses the Routing :doc:`middleware </fundamentals/middleware>` to match the URLs of incoming requests and map them to actions. Routes are defined in startup code or attributes. Routes describe how URL paths should be matched to actions. Routes are also used to generate URLs (for links) sent out in responses.

This document will explain the interactions between MVC and routing, and how typical MVC applications make use of routing features. See :doc:`Routing </fundamentals/routing>` for details on advanced routing.

Setting up Routing Middleware
------------------------------

In your `Configure` method you may see code similar to::

  app.UseMvc(routes =>
  {
     routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
  });

Inside the call to :dn:method:`~Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions.UseMvc`, :dn:method:`~Microsoft.AspNetCore.Builder.MapRouteRouteBuilderExtensions.MapRoute` is used to create a single route, which we'll refer to as the ``default`` route. Most MVC applications will use a route with a template similar to the ``default`` route.

The route template ``"{controller=Home}/{action=Index}/{id?}"`` can match a URL path like ``/Products/Details/5`` and will extract the route values ``{ controller = Products, action = Details, id = 5 }`` by tokenizing the path. MVC will attempt to locate a controller named ``ProductsController`` and run the action ``Details``::

  public class ProductsController : Controller
  {
     public IActionResult Details(int id) { ... }
  }

Note that in this example, model binding would use the value of ``id = 5`` to set the ``id`` parameter to ``5`` when invoking this action. See the :doc:`/mvc/models/model-binding` for more details.

Using the ``default`` route::

   routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

The route template:

- ``{controller=Home}`` defines ``Home`` ad the default  ``controller``
- ``{action=Index}`` defines ``Index`` as the default ``action``
- ``{id?}`` defines the ``id`` as optional

Default and optional route parameters do not need to be present in the URL path for a match. See the  :doc:`Routing </fundamentals/routing>` for a detailed description of route template syntax.

``"{controller=Home}/{action=Index}/{id?}"`` can match the URL path ``/`` and will produce the route values ``{ controller = Home, action = Index }``. The values for ``controller`` and ``action`` make use of the default values, ``id`` does not produce a value since there is no corresponding segment in the URL path. MVC would use these route values to select the ``HomeController`` and ``Index`` action::

  public class HomeController : Controller
  {
    public IActionResult Index() { ... }
  }

Using this controller definition and route template, the ``HomeController.Index`` action would be executed for any of the following URL paths:

- ``/Home/Index/17``
- ``/Home/Index``
- ``/Home``
- ``/``

The convenience method :dn:method:`~Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions.UseMvcWithDefaultRoute`::

  app.UseMvcWithDefaultRoute();

Can be used to replace::

  app.UseMvc(routes =>
  {
     routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
  });

``UseMvc`` and ``UseMvcWithDefaultRoute`` add an instance of :dn:cls:`~Microsoft.AspNetCore.Builder.RouterMiddleware` to the middleware pipeline. MVC doesn't interact directly with middleware, and uses routing to handle requests. MVC is connected to the routes through an instance of :dn:cls:`~Microsoft.AspNetCore.Mvc.Internal.MvcRouteHandler`. The code inside of ``UseMvc`` is similar to the following::

   var routes = new RouteBuilder(app);

   // Add connection to MVC, will be hooked up by calls to MapRoute.
   routes.DefaultHandler = new MvcRouteHandler(...);

   // Execute callback to register routes.
   // routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

   // Create route collection and add the middleware.
   app.UseRouter(routes.Build());

**[review: this section rewritten]**
:dn:method:`~Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions.UseMvc` does not directly define any routes, it adds a placeholder to the route collection for the ``attribute`` route. The overload ``UseMvc(Action<IRouteBuilder>)`` lets you add your own routes and also supports attribute routing. :dn:method:`~Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions.UseMvcWithDefaultRoute` defines a default route and supports attribute routing. The :ref:`attribute-routing-ref-label` section includes more details on attribute routing.

Conventional routing
---------------------

The ``default`` route::

  routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

is an example of a *conventional routing*. We call this style *conventional routing* because it establishes a *convention* for URL paths:

-  the first path segment maps to the controller name
-  the second maps to the action name.
-  the third segment is used for an optional ``id`` used to map to a model entity

Using this ``default`` route, the URL path ``/Products/List`` maps to the ``ProductsController.List`` action, and ``/Blog/Article/17`` maps to ``BlogController.Article``. This mapping is based on the controller and action names **only** and is not based on namespaces, source file locations, or method parameters.

.. Tip:: Using conventional routing with the default route allows you to build the application quickly without having to come up with a new URL pattern for each action you define. For an application with CRUD style actions, having consistency for the URLs across your controllers can help simplify [things - Edit replaced with following] your code and make your UI more predictable.

.. warning: The ``id`` is defined as optional by the route template, meaning that your actions can execute *without* the id provided as part of the URL. [Original - see replacement below: Usually what will happen if ``id`` is omitted from the URL is that it will be set to ``0`` by model binding, and as a result no entity will be found in the database matching ``id == 0``.][{are there exceptions, for example can ambient values be used - if so we need to list exceptions and tell them they need to use attribute routing to prevent getting the wrong ID} ] Attribute routing can give you fine-grained control to make the id required for some actions and not for others. By convention the documentation will include optional parameters like ``id`` when they are likely to appear in correct usage.

You can make the ``id`` parameter optional to verify the ``id`` value has been provided in the URL::

   public IActionResult Index(int? id)
   {
      if (id == null)
      {
         HandleNullId();
      }
      else
      {
       // ...
      }
      return View();
   }

Multiple Routes
-------------------

You can add multiple routes inside ``UseMvc`` by adding more calls to ``MapRoute``. Doing so allows you to define multiple conventions, or to add conventional routes that are dedicated to a specific action, such as::

   app.UseMvc(routes =>
   {
      routes.MapRoute("blog", "blog/{*article}", defaults: new { controller = "Blog", action = "Article" });
      routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
   }

The ``blog`` route here is a *dedicated conventional route*, meaning that it uses the conventional routing system, but is dedicated to a specific action. Since ``controller`` and ``action`` don't appear in the route template as parameters, they can only have the default values, and thus this route will always map to the action ``BlogController.Article``.

Routes in the route collection are ordered, and will be processed in the order they are added. So in this example, the ``blog`` route will be tried before the ``default`` route.

.. note:: Note that *dedicated conventional routes* often use catch-all route parameters like ``{*article}`` to capture the remaining portion of the URL path. This can make a route 'too greedy' meaning that it matches URLs that you intended to be matched by other routes. Put the 'greedy' routes later in the route table to solve this.

Fallback
----------

As part of request processing, MVC will verify that the route values can be used to find a controller and action in your application. If the route values don't match an action then the route is not considered a match, and the next route will be tried. This is called *fallback*, and it's intended to simplify cases where conventional routes overlap.

Disambiguating Actions
------------------------

When two actions match through routing, MVC must disambiguate to choose the 'best' candidate or else throw an exception. For example::

   public class ProductsController : Controller
   {
      public IActionResult Edit(int id) { ... }

      [HttpPost]
      public IActionResult Edit(int id, Product product) { ... }
   }

This controller defines two actions that would match the URL path ``/Products/Edit/17`` and route data ``{ controller = Products, action = Edit, id = 17 }``. This is a typical pattern for MVC controllers where ``Edit(int)`` shows a form to edit a product, and ``Edit(int, Product)`` processes  the posted form. To make this possible MVC would need to choose ``Edit(int, Product)`` when the request is an HTTP ``POST`` and ``Edit(int)`` when the HTTP verb is anything else.

The :dn:cls:`~Microsoft.AspNetCore.Mvc.HttpPostAttribute` is an implementation of :dn:iface:`~Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint` that will only allow the action to be selected when the HTTP verb is ``POST``. The presence of an ``IActionConstraint`` makes the ``Edit(int, Product)`` a 'better' match than ``Edit(int)``, so ``Edit(int, Product)`` will be tried first. See :ref:`iactionconstraint-ref-label` for details.

You will only need to write custom ``IActionConstraint`` implementations in specialized scenarios, but it's important to understand the role of attributes like ``HttpPostAttribute``  - similar attributes are defined for other HTTP verbs. In conventional routing it's common for actions to use the same action name when they are part of a ``show form -> submit form`` workflow. The convenience of this pattern will become more apparent after reviewing :ref:`url-gen-ref-label`.

If multiple routes match, and MVC can't find a 'best' route, it will throw an :dn:cls:`~Microsoft.AspNetCore.Mvc.Internal.AmbiguousActionException`.

.. _routing-route-name-ref-label:

Route Names
-------------

The strings  ``"blog"`` and ``"default"`` in the following examples are route names::

  app.UseMvc(routes =>
  {
     routes.MapRoute("blog", "blog/{*article}",
                 defaults: new { controller = "Blog", action = "Article" });
     routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
  });

The route names give the route a logical name so that the named route can be used for URL generation. This greatly simplifies URL creation when the ordering of routes could make URL generation complicated. Routes names must be unique application-wide.

Route names have no impact on URL matching or handling of requests; they are used only for URL generation. :doc:`Routing </fundamentals/routing>` has more detailed information on URL generation including URL generation in MVC-specific helpers.

.. _attribute-routing-ref-label:

Attribute Routing
-------------------------

.. review required: Your original does not match About/Contact so I think this might be a better example.

Attribute routing uses a set of attributes to map actions directly to route templates. In the following example, ``app.UseMvc();`` is used in the ``Configure`` method and no route is passed. The ``HomeController`` will match a set of URLs similar to what the default route ``{controller=Home}/{action=Index}/{id?}`` would match:

.. code-block:: c#

  public class HomeController : Controller
  {
     [Route("")]
     [Route("Home")]
     [Route("Home/Index")]
     public IActionResult Index()
     {
        return View();
     }
     [Route("Home/About")]
     public IActionResult About()
     {
        return View();
     }
     [Route("Home/Contact")]
     public IActionResult Contact()
     {
        return View();
     }
  }

The ``HomeController.Index()`` action will be executed for any of the URL paths ``/``, ``/Home``, or ``/Home/Index``.

.. note:: This example highlights a key programming difference between attribute routing and conventional routing. Attribute routing requires more input to specify something the conventional default route handles more succintly. However, attribute routing allows (and requires) precise control of which route templates apply to each action.

Note that with attribute routing the controller name and action names play **no** role in which action is selected. This example will match the same URLs as the previous example.

.. code-block:: c#

  public class MyDemoController : Controller
  {
     [Route("")]
     [Route("Home")]
     [Route("Home/Index")]
     public IActionResult MyIndex()
     {
        return View("Index");
     }
     [Route("Home/About")]
     public IActionResult MyAbout()
     {
        return View("About");
     }
     [Route("Home/Contact")]
     public IActionResult MyContact()
     {
        return View("Contact");
     }
  }

.. note:: These route templates don't define route parameters for ``action``, ``area``, and ``controller``. In fact, these route parameters are not allowed in attribute routes. Since the route template is already assocated with an action, it wouldn't make sense to parse the action name from the URL.

Attribute routing can also make use of the ``HTTP[Verb]`` attributes such as :dn:cls:`~Microsoft.AspNetCore.Mvc.HttpPostAttribute`. All of these attributes can accept a route template. This example shows two actions that match the same route template:

.. review: changed CreateProduct from Put to Post

.. code-block:: c#

   [HttpGet("/products")]
   public IActionResult ListProducts()
   {
      // ...
   }

   [HttpPost("/products")]
   public IActionResult CreateProduct(...)
   {
      // ...
   }

For a URL path like ``/products`` the ``ProductsApi.ListProducts`` action will be executed when the HTTP verb is ``GET`` and ``ProductsApi.CreateProduct`` will be executed when the HTTP verb is ``POST``. Attribute routing first matches the URL against the set of route templates defined by route attributes. Once a route template matches,   :dn:iface:`~Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraint` constraints are applied to determine which actions can be executed.

.. Tip:: When building a REST API, it's rare that you will want to use ``[Route(...)]`` on an action method. It's better to use the more specific ``Http*Verb*Attributes`` to be precise about what your API supports. Clients of REST APIs are expected to know what paths and HTTP verbs map to specific logical operations.

Since an attribute route applies to a specific action, it's easy to make parameters required as part of the route template definition. In this example, the id is required as part of the URL path.

.. code-block:: c#

   public class ProductsApiController : Controller
   {
      [HttpGet("/products/{id}", Name = "Products_List")]
      public IActionResult GetProduct(int id) { ... }
   }

The ``ProductsApi.GetProducts(int)`` action will be executed for a URL path like ``/products/3`` but not for a URL path like ``/products``. See :doc:`Routing </fundamentals/routing>` for a full description of route templates and related options.

This route attribute also defines a *route name* of ``Products_List``. Route names can be used to generate a URL based on a specific route. Route names have no impact on the URL matching behavior of routing and are only used for URL generation. Route names must be unique application-wide.

.. note:: Contrast this with the conventional *default route*, which defines the ``id`` parameter as optional (``{id?}``). This ability to precisly specify APIs has advantages, such as  allowing ``/products`` and ``/products/5`` to be dispatched to different actions.

.. _routing-combining-ref-label:

Combining Routes
-----------------

To make attribute routing less repetative, route attributes on the controller are combined with route attributes on the individual actions. Any route templates defined on the controller are preprended to route templates on the actions. Placing a route attribute on the controller makes **all** actions in the controller use attribute routing.

.. code-block:: c#

   [Route("products")]
   public class ProductsApiController : Controller
   {
      [HttpGet]
      public IActionResult ListProducts() { ... }

      [HttpGet("{id}")]
      public ActionResult GetProduct(int id) { ... }
   }

In this example the URL path ``/products`` can match ``ProductsApi.ListProducts``, and the URL path ``/products/5`` can match ``ProductsApi.GetProduct(int)``. Both of these actions only match HTTP ``GET`` becase they are decorated with the :dn:cls:`~Microsoft.AspNetCore.Mvc.HttpGetAttribute`.

Route templates applied to an action that begin with a ``/`` do not get combined with route templates applied to the controller. This example matches a set of URL paths similar to the *default route*.

.. literalinclude:: routing/sample/Controllers/HomeController.cs
  :language: c#
  :start-after: snippet
  :end-before: #endregion

.. _routing-ordering-ref-label:

Ordering attribute routes
--------------------------

In contrast to conventional routes which execute in a defined order, attribute routing builds a tree and matches all routes simultaneously. This behaves as-if the route entries where placed in an ideal ordering; the most specific routes have a chance to execute before the more general routes.

For example, a route like ``blog/search/{topic}`` is more specific than a route like ``blog/{*article}``. Logically speaking the ``blog/search/{topic}`` route 'runs' first, by default, because that's the only sensible ordering. Using conventional routing, the developer is  responsible for placing routes in the desired order.

.. review: I added The default order is ``0``

Attribute routes can configure an order, using the ``Order`` property of all of the framework provided route attributes. Routes are processed according to an ascending sort of the ``Order`` property. The default order is ``0``. Setting a route using ``Order = -1`` will run before routes that don't set an order. Setting a route using ``Order = 1`` will run after default route ordering.

.. Tip:: Avoid depending on ``Order``. If your URL-space requires explicit order values to route correctly, then it's likely confusing to clients as well. In general attribute routing will select the correct route with URL matching. If the default order used for URL generation isn't working, using route name as an override is usually simpler than applying the ``Order`` property.

.. _routing-token-replacement-templates-ref-label:

Token replacement in route templates ([controller], [action], [area])
-----------------------------------------------------------------------

For convenience, attribute routes support *token replacement* by enclosing a token in square-braces (``[``, ``]``]). The tokens ``[action]``, ``[area]``, and ``[controller]`` will be replaced with the values of the action name, area name, and controller name from the action where the route is defined. In this example the actions can match URL paths as described in the comments:

.. literalinclude:: routing/sample/Controllers/ProductsController.cs
  :language: c#
  :lines: 7-11,13-17,20-22
  :dedent: 4

Token replacement occurs as the last step of building the attribute routes. The above example will behave the same as the following code:

.. literalinclude:: routing/sample/Controllers/ProductsController2.cs
  :language: c#
  :lines: 7-11,13-17,20-22
  :dedent: 4

Attribute routes can also be combined with inheritance. This is particularly powerful combined with token replacement.

.. code-block:: c#

   [Route("api/[controller]")]
   public abstract class MyBaseController : Controller { ... }

   public class ProductsController : MyBaseController
   {
      [HttpGet] // Matches '/api/Products'
      public IActionResult List() { ... }

      [HttpPost("{id}")] // Matches '/api/Products/{id}'
      public IActionResult Edit(int id) { ... }
   }

Token replacement also applies to route names defined by attribute routes. ``[Route("[controller]/[action]", Name="[controller]_[action]")]`` will generate a unique route name for each action.

.. _routing-multiple-routes-ref-label:

Multiple Routes
-------------------

Attribute routing supports defining multiple routes that reach the same action. The most common usage of this is to mimic the behavior of the *default conventional route* as shown in the following example:

.. code-block:: c#

   [Route("[controller]")]
   public class ProductsController : Controller
   {
      [Route("")]     // Matches 'Products'
      [Route("Index")] // Matches 'Products/Index'
      public IActionResult Index()
   }

Putting multiple route attributes on the controller means that each one will combine with each of the route attributes on the action methods.

.. code-block:: c#

   [Route("Store")]
   [Route("[controller]")]
   public class ProductsController : Controller
   {
      [HttpPost("Buy")]     // Matches 'Products/Buy' and 'Store/Buy'
      [HttpPost("Checkout")] // Matches 'Products/Checkout' and 'Store/Checkout'
      public IActionResult Buy()
   }

When multiple route attributes (that implement ``IActionConstraint``) are placed on an action, then each action constraint combines with the route template from the attribute that defined it.

.. code-block:: c#

   [Route("api/[controller]")]
   public class ProductsController : Controller
   {
      [HttpPut("Buy")]      // Matches PUT 'api/Products/Buy'
      [HttpPost("Checkout")] // Matches POST 'api/Products/Checkout'
      public IActionResult Buy()
   }

.. Tip:: While using multiple routes on actions can seem powerful, it's better to keep your application's URL space simple and well-defined. Use multiple routes on actions only where needed, for example to support existing clients.

.. _routing-cust-rt-attr-irt-ref-label:

Custom route attributes using ``IRouteTemplateProvider``
---------------------------------------------------------

All of the route attributes provided in the framework (`[Route(...)]`, `[HttpGet(...)]`, etc.) implement the :dn:iface:`~Microsoft.AspNetCore.Mvc.Routing.IRouteTemplateProvider`
`IRouteTemplateProvider` interface. MVC looks for attributes on controller classes and action methods when the app starts and uses the ones that implement ``IRouteTemplateProvider`` to build the intial set of routes.

You can implement ``IRouteTemplateProvider`` to define your own route attributes, but each ``IRouteTemplateProvider`` only allows you to define a single route with a custom route template, order, and name:

.. code-block:: c#

  public class MyApiControllerAttribute : Attribute, IRouteTemplateProvider
  {
     public string Template => "api/[controller]";

     public int? Order { get; set; }

     public string Name { get; set; }
  }

The attribute from the above example automatically sets the ``Template`` to ``"api/[controller]"`` wherever ``[MyApiController]`` is used.

.. _routing-app-model-ref-label:

Using Application Model to customize attribute routes
------------------------------------------------------

The *application model* is an object model created at startup with all of the metadata used by MVC to route and execute your actions. The *application model* includes all of the data gathered from route attributes (through ``IRouteTemplateProvider``). You can write *conventions* to modify the application model at startup time to customize how routing behaves. This section shows a simple example of customizing routing using application model.

.. review: removed: See the application model section for detailed documentation. (we don't seem to have an application model section)

.. literalinclude:: routing/sample/NamespaceRoutingConvention.cs
  :language: c#


.. _routing-mixed-ref-label:

Mixed Routing
--------------

MVC applications can mix the use of conventional routing and attribute routing. It's possible, for instance, to use conventional routes for controllers serving HTML pages for browsers, and attribute routing for controllers serving REST APIs.

Actions are either conventionally routed or attribute routed. Placing a route on the controller or the action makes it attribute routed. Actions that define attribute routes cannot be reached through the conventional routes and vice-versa. **Any** route attribute on the controller makes all actions in the controller attribute routed.

.. Note:: What distinguishes the two types of routing systems is the process applied after a URL matches a route template. In conventional routing, the route values from the match are used to choose the action and controller from a lookup table of all conventional routed actions. In attribute routing, each template is already associated with an action, and no further lookup is needed.

.. _routing-url-gen-ref-label:

URL Generation
---------------

MVC applications can use routing's URL generation features to generate URL links to actions. Generating URLs eliminates hardcoding URLs, making your code more robust and maintainable. This section focuses on the URL generation features provided by MVC and will only cover basics of how URL generation works. See :doc:`Routing </fundamentals/routing>` for a detailed description of URL generation.

The :dn:iface:`~Microsoft.AspNetCore.Mvc.IUrlHelper` interface is the underlying piece of infrastructure between MVC and routing for URL generation. You'll find an instance of ``IUrlHelper`` available through the ``Url`` property in controllers, views, and view components.

In this example, the ``IUrlHelper`` interface is used through the ``Controller.Url`` property to generate a URL to another action.

.. literalinclude:: routing/sample/Controllers/UrlGenerationController.cs

If the application is using the default conventional route, the value of the ``url`` variable will be the URL path string ``/UrlGeneration/Destination``. This URL path is created by routing by combining the route values from the current request (ambient values), with the values passed to ``Url.Action`` and substituting those values into the route template::

   ambient values: { controller = "UrlGeneration", action = "Source" }
   values passed to Url.Action: { controller = "UrlGeneration", action = "Destination" }
   route template: {controller}/{action}/{id?}

   result: /UrlGeneration/Destination

Each route parameter in the route template has its value substituted by matching names with the values and ambient values. A route parameter that does not have a value can use a default value if it has one, or be skipped if it is optional (as in the case of ``id`` in this example). URL generation will fail if any required route parameter doesn't have a corresponding value. If URL generation fails for a route, the next route is tried until all routes have been tried or a match is found.

The example of ``Url.Action`` above assumes conventional routing, but URL generation works similarly with attribute routing, though the concepts are different. With conventional routing, the route values are used to expand a template, and the route values for ``controller`` and ``action`` usually appear in that template - this works because the URLs matched by routing adhere to a *convention*. In attribute routing, the route values for ``controller`` and ``action`` are not allowed to appear in the template - they are instead used to look up which template to use.

This example uses attribute routing:

.. literalinclude:: routing/sample/StartupUseMvc.cs
  :language: c#
  :start-after: snippet_1
  :end-before: #endregion

.. literalinclude:: routing/sample/Controllers/UrlGenerationControllerAttr.cs
  :language: none
  :start-after: snippet_1
  :end-before: #endregion

MVC builds a lookup table of all attribute routed actions and will match the ``controller`` and ``action`` values to select the route template to use for URL generation. In the sample above,   ``custom/url/to/destination`` is generated.

Generating URLs by action name
--------------------------------

``Url.Action`` (:dn:iface:`~Microsoft.AspNetCore.Mvc.IUrlHelper` . :dn:method:`~Microsoft.AspNetCore.Mvc.IUrlHelper.Action`) and all related overloads all are based on that idea that you want to specify what you're linking to by specifying a controller name and action name.

.. note:: When using ``Url.Action``, the current route values for ``controller`` and ``action`` are specified for you - the value of ``controller`` and ``action`` are part of both *ambient values* **and** *values*. The method ``Url.Action``, always uses the current values of ``action`` and ``controller`` and will generate a URL path that routes to the current action.

Routing attempts to use the values in ambient values to fill in information that you didn't provide when generating a URL. Using a route like ``{a}/{b}/{c}/{d}`` and ambient values ``{ a = Alice, b = Bob, c = Carol, d = David }``, routing has enough information to generate a URL without any additional values - since all route paramaters have a value. If you added the value ``{ d = Donovan }``, the the value ``{ d = David }`` would be ignored, and the generated URL path would be ``Alice/Bob/Carol/Donovan``. 

.. warning:: URL paths are hierarchical. In the example above, if you added the value ``{ c = Cheryl }``, both of the values ``{ c = Carol, d = David }`` would be ignored. In this case we no longer have a value for ``d`` and URL generation will fail. You would need to specify the desired value of ``c`` and ``d``.  You might expect to hit this problem with the default route (``{controller}/{action}/{id?}``) - but you will rarely encounter this behavior in practice as ``Url.Action`` will always explicitly specify a ``controller`` and ``action`` value.

Longer overloads of ``Url.Action`` also take an additional *route values* object to provide values for route parameters other than ``controller`` and ``action``. You will most commonly see this used with ``id`` like ``Url.Action("Buy", "Products", new { id = 17 })``. By convention the *route values* object is usually an object of anonymous type, but it can also be an ``IDictionary<>`` or a 'plain old .NET object'. Any additional route values that don't match route parameters are put in the query string.

.. literalinclude:: routing/sample/Controllers/TestController.cs
  :language: c#


.. tip: To create an absolute URL, use an overload that accepts a ``protocol`` like: ``Url.Action("Buy", "Products", new { id = 17 }, protocol: Request.Scheme)``

### Generating URLs by route














----------------------------------------------------------------------------


### Custom route attributes using IRouteTemplateProvider







.. _iactionconstraint-ref-label:

Understanding IActionConstraint
---------------------------------

bla bla bal  ````````````````

more bla



more bla

.. _url-gen-ref-label:

URL generation
--------------------

bla bla bal

more bla
zz

