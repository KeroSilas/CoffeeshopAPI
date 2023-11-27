﻿using AutoMapper;
using Business.Service;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public OrderController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route ("GetOrders")]
    public ActionResult<IEnumerable<Order>> GetOrders()
    {
        var orders = _orderService.GetOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public ActionResult<Order> GetOrder(Guid id)
    {
        var order = _orderService.GetOrder(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPut("{id}")]
    public ActionResult<Order> UpdateOrder(Guid id, Order order)
    {
        if (id != order.Id)
        {
            return BadRequest();
        }

        if (!_orderService.OrderExists(id))
        {
            return NotFound();
        }

        var updatedOrder = _orderService.UpdateOrder(id, order);
        return Ok(updatedOrder);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteOrder(Guid id)
    {
        _orderService.DeleteOrder(id);
        return Ok();
    }

    [HttpPost]
    public ActionResult<Order> CreateOrder(Order order)
    {
        var createdOrder = _orderService.CreateOrder(order);
        return Ok(createdOrder);
    }
}